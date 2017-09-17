import {Figure, FigureStyle} from './../../model/Figure'
import {Image} from './../../model/Image'
import {PointStyle} from './../../model/Point'
import {SimplePoint} from './../../model/SimplePoint'
import fetch from 'isomorphic-fetch'
import Pen from './../../model/tools/Pen'
import Presentation from './../../model/tools/Presentation'
import Eraser from './../../model/tools/Eraser'
import Move from './../../model/tools/Move'
import BoardData from './../../model/BoardData'

const protocol = 'http'
const domain = 'qipserverapi.azurewebsites.net'

function apiFetch (resource, options) {
  return fetch(`${protocol}://${domain}/api/${resource}`, options)
}

export default class PersistLS {
  static _configureWSProtocol = function (socket, grid, canvasContext) {
    socket.onmessage = (event) => {
      const {type, payload} = JSON.parse(event.data)
      console.log('received ws message of type ' + type)
      switch (type) {
        case 'CREATE_LINE':
          if (payload.tempId != null) {
            // update figure
            const prevFigure = grid.getFigure(payload.tempId)
            // update point's figures of updated figures
            prevFigure.points.forEach(point => {
              const pointStyle = point.getStyleOf(payload.tempId)
              point.removeFigure(payload.tempId)
              point.addFigure(payload.id, pointStyle)
            })
            // update figure id and history
            grid.updateHistoryFigureId(prevFigure.id, payload.id)
            prevFigure.id = payload.id

            // update grid map of figures
            grid.updateMapFigure(payload.tempId, prevFigure)

            console.log('updated line with id ' + payload.tempId + ' to id ' + payload.id)
            return
          }
          const figStyle = new FigureStyle(payload.figure.Style.Color, 1)
          const newFigure = new Figure(figStyle, payload.figure.Id)
          // todo: this map wouldn't be necessary if server has the same model as client
          newFigure.points = payload.figure.Points.map(serverPoint => {
            const simplePointStyle = new PointStyle(serverPoint.Style.Width)
            return new SimplePoint(serverPoint.X, serverPoint.Y, simplePointStyle, serverPoint.Idx)
          })
          grid.addFigure(newFigure)
          grid.draw(canvasContext, 1)
          console.log('received new line with id ' + payload.figure.Id)
          break
        case 'DELETE_LINE':
          const figureToDelete = grid.getFigure(payload.id)
          grid.removeFigure(figureToDelete, canvasContext, 1)
          break
        case 'ALTER_LINE':
          // todo: por estes comentários em vez de apagar e criar a figura quando o servidor estiver a enviar o offsetPoint
          const figureToMove = grid.getFigure(payload.figure.Id)
          const isScaling = payload.isScaling
          if (isScaling !== 'False') {
            const offsetPoint = {x: payload.offsetPoint.X, y: payload.offsetPoint.Y} // translate from server model
            figureToMove.scale(isScaling, offsetPoint, grid, canvasContext)
            grid.draw(canvasContext, 1)
          } else {
            grid.moveLine(figureToMove, point => {
              return grid.getOrCreatePoint(point.x + payload.offsetPoint.X, point.y + payload.offsetPoint.Y)
            }, canvasContext, 1)
          }
          break
        case 'CREATE_IMAGE':
          if (payload.tempId != null) {
            const prevFigure = grid.getFigure(payload.tempId)
            grid.updateHistoryFigureId(prevFigure.id, payload.id)
            prevFigure.id = payload.id
            grid.updateMapFigure(payload.tempId, prevFigure)
            console.log('updated image with id ' + payload.tempId + ' to id ' + payload.id)
            grid.draw(canvasContext, 1)
            return
          }
          const newImage = new Image({x: payload.figure.Origin.X, y: payload.figure.Origin.Y}, payload.figure.Src, payload.figure.Width, payload.figure.Height, payload.figure.Id, () => grid.draw(canvasContext, 1))
          grid.addImage(newImage)
          grid.draw(canvasContext, 1)
          break
        case 'DELETE_IMAGE':
          grid.removeImage(payload.id, canvasContext, 1)
          break
        case 'ALTER_IMAGE':
          console.log(payload)
          const prevFigure = grid.getFigure(payload.figure.Id)
          prevFigure.setSrcPoint(new SimplePoint(payload.figure.Origin.X, payload.figure.Origin.Y))
          prevFigure.setWidth(payload.figure.Width)
          prevFigure.setHeight(payload.figure.Height)
          grid.draw(canvasContext, 1)
          break
        case 'POINT_TO':
          grid.draw(canvasContext, 1)
          if (payload.point.X > 0 && payload.point.Y > 0) {
            Presentation._drawPointer(canvasContext, payload.point.X, payload.point.Y)
          }
          break
        case 'BOARD_CLEAN':
          grid.resetHistory()
          grid.getFiguresArray().forEach(figure => grid.removeFigure(figure.id, canvasContext, 1, false))
          grid.clean(canvasContext)
          break
      }
    }
  }

  // todo: enviar também os favoritos, current tool e prev tools
  // this procedure sends to server all data stored in local storage.
  // it should be called when user authenticates but there was already some information in local storage
  static _persistBoardByWS = function (socket, boardId) {
    const figures = JSON.parse(window.localStorage.getItem('figures'))
    figures.forEach(fig => {
      // Extract id from fig and spread all other properties to figure
      const { id, ...figure } = fig
      const payload = {
        ...figure,
        tempId: id,
        boardId
      }
      const objToSend = {
        type: fig.type === 'figure' ? 'CREATE_LINE' : 'CREATE_IMAGE',
        payload
      }
      socket.send(JSON.stringify(objToSend))
    })
  }

  // get initial board from server by web sockets
  static _getInitialBoardWS = function (boardId, accessToken) {
    // todo: fazer também um fetch ao board específico e obter o maxFigureId em vez de estar a verificar qual o maior atrvés das figuras recebidas
    console.info('getting board data from server by web sockets')
    // fetch data of current board
    return Promise.all([
      apiFetch(`boards/${boardId}/figures/lines`, {
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${accessToken}`
        },
        method: 'GET'
      }).then(figRes => {
        if (figRes.status >= 400) {
          throw new Error('Bad response from server. Check if Board Id is correct')
        }
        return figRes.json()
      }),
      apiFetch(`boards/${boardId}/figures/images`, {
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${accessToken}`
        },
        method: 'GET'
      }).then(imgRes => {
        if (imgRes.status >= 400) {
          throw new Error('Bad response from server. Check if Board Id is correct')
        }
        return imgRes.json()
      })
    ]).then(allFigs => {
      const images = allFigs[1].map(img => { // sync with server model
        img.srcPoint = img.origin
        return img
      })
      const figures = allFigs[0].concat(images)
      let maxId = -1
      console.log('figures fetched from initial board:')
      console.log(figures)

      const initBoard = {
        grid: {figures, maxId},
        canvasSize: { // todo: get from server
          width: 0,
          height: 0
        }
      }

      return initBoard
    })
  }

  static _cleanCanvasWS = function (boardId, maxFigureId, socket) {
    socket.send(JSON.stringify({
      type: 'BOARD_CLEAN',
      payload: {boardId, maxFigureId}
    }))
  }

  static _updateCanvasSizeWS = function (canvasSize) {}

  static _updateUserPreferencesWS = function (updatedPreferences, profile, accessToken) {
    // preferences has
    // favorites | pen colors | defaultPen
    // DefaultEraser | CurrTool | Settings

    return apiFetch(`users/${profile.sub}/preferences`, {
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accessToken}`
      },
      method: 'PUT',
      body: JSON.stringify({
        UserId: profile.sub,
        Favorites: JSON.stringify(updatedPreferences.favorites),
        PenColors: JSON.stringify(updatedPreferences.penColors),
        DefaultPen: JSON.stringify(updatedPreferences.defaultPen),
        DefaultEraser: JSON.stringify(updatedPreferences.defaultEraser),
        CurrTool: JSON.stringify(updatedPreferences.currTool),
        Settings: JSON.stringify(updatedPreferences.settings)
      })
    }).then(updatedPreferencesRes => {
      if (updatedPreferencesRes.status >= 400) {
        throw new Error('Bad response from server. Check if Board Id is correct')
      }
      return updatedPreferencesRes
    })
  }

  static _addUserBoardWS = function (boardName, user, accessToken) {
    console.info('adding new board: ' + boardName)
    const headers = {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    }
    accessToken != null && (headers.Authorization = `Bearer ${accessToken}`)
    // this fetch will create a board and associate it to current user. Current user will be admin
    return apiFetch(`boards/`, {
      headers: headers,
      method: 'POST',
      body: JSON.stringify({
        name: boardName,
        maxDistPoints: 0
      })
    }).then(addedBoardRes => {
      if (addedBoardRes.status >= 400) {
        throw new Error('Bad response from server. Check if Board Id is correct')
      }
      return addedBoardRes.json()
    }).then(addedBoard => {
      return addedBoard
    })
  }

  // get user info from server by web sockets
  static _getUserInfoAsyncWS = function (grid, profile, accessToken) {
    console.info('getting user info from server by web sockets')
    // fetch data of current board
    return Promise.all([
      apiFetch(`users/${profile.sub}/preferences`, {
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${accessToken}`
        },
        method: 'GET'
      }).then(preferencesRes => {
        if (preferencesRes.status >= 400) {
          throw new Error('Bad response from server. Check if User Sub is correct')
        }
        if (preferencesRes.status === 204) { // .json cannot handle response without content and server has not a default preferences response
          return {
            favorites: '[]',
            currTool: 'null',
            settings: '[]',
            defaultEraser: 'null',
            defaultPen: 'null'
          }
        }
        return preferencesRes.json()
      }),
      apiFetch(`users/${profile.sub}/boards`, {
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${accessToken}`
        },
        method: 'GET'
      }).then(userBoardsRes => {
        if (userBoardsRes.status >= 400) {
          throw new Error('Bad response from server. Check if User Sub is correct')
        }
        return userBoardsRes.json()
      })
    ]).then(allRes => {
      const favorites = JSON.parse(allRes[0].favorites)
        .map(fav => this._getToolFromWS(grid, fav))
      const userInfo = {
        defaultPen: null,
        defaultEraser: null,
        currTool: this._getToolFromWS(grid, JSON.parse(allRes[0].currTool)),
        favorites,
        userBoards: allRes[1],
        settings: JSON.parse(allRes[0].settings)
      }
      return userInfo
    })
  }

  static _getToolFromWS = function (grid, rawTool) {
    if (rawTool != null) {
      switch (rawTool.type) {
        case ('pen'):
          return new Pen(grid, rawTool.color, rawTool.width)
        case ('eraser'):
          return new Eraser(grid, rawTool.width, rawTool.eraserType)
        case ('move'):
          return new Move(grid)
      }
    }
  }

  // get board info from server by web sockets
  static _getBoardInfoWS = function (boardId, profile, accessToken) {
    console.info('getting board info from server by web sockets')
    // if user is not authenticated, user permission is public permission so there is no need to do second fetch
    const promises = [
      apiFetch(`boards/${boardId}`, {
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${accessToken}`
        },
        method: 'GET'
      }).then(boardInfoRes => {
        if (boardInfoRes.status >= 400) {
          throw new Error('Bad response from server. Check if Board Id is correct')
        }
        return boardInfoRes.json()
      })]

    if (profile != null) {
      promises.push(
        apiFetch(`users/${profile.sub}/boards/${boardId}`, {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
          },
          method: 'GET'
        }).then(boardInfoRes => {
          if (boardInfoRes.status >= 400) {
            throw new Error('Bad response from server. Check if Board Id is correct')
          }
          return boardInfoRes.json()
        })
      )
    }
    // fetch data of board
    return Promise.all(promises)
      .then(allRes => {
        const boardInfo = allRes[0]
        const userBoardInfo = allRes.length === 1 ? {permission: boardInfo.basePermission} : allRes[1] // if user is not authenticated, board permissions is 0
        return new BoardData(boardInfo.id, boardInfo.name, boardInfo.basePermission, userBoardInfo.permission)
      })
  }

  static _getBoardUsers = function (boardId, accessToken) {
    return apiFetch(`boards/${boardId}/usersboards`, {
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accessToken}`
      },
      method: 'GET'
    }).then(boardUsersRes => {
      if (boardUsersRes.status >= 400) {
        throw new Error('Bad response from server.')
      }
      return boardUsersRes.json()
    }).then(boardUsers => {
      return boardUsers
    })
  }

  static _updateBoardBasePermissionWS = function (boardId, boardName, basePermission, accessToken) {
    return apiFetch(`boards/${boardId}`, {
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accessToken}`
      },
      method: 'PUT',
      body: JSON.stringify({
        id: boardId,
        name: boardName,
        basePermission: basePermission,
        maxDistPoints: 0
      })
    }).then(usersRes => {
      if (usersRes.status >= 400) {
        throw new Error('Bad response from server.')
      }
      return usersRes
    })
  }

  static _createUsersPermissionWS = function (users, boardId, usersPermission, accessToken) {
    const promises = []
    users.forEach(userId => {
      promises.push(
        apiFetch(`boards/${boardId}/usersboards`, {
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
          },
          method: 'POST',
          body: JSON.stringify({
            userId: userId,
            boardId,
            permission: usersPermission
          })
        }).then(updatedPermissionRes => {
          if (updatedPermissionRes.status >= 400) {
            throw new Error('Bad response from server.')
          }
          return updatedPermissionRes.json()
        })
      )
    })
    return Promise.all(promises)
  }

  static _updateUserPermissionWS = function (userId, boardId, userPermission, accessToken) {
    return apiFetch(`boards/${boardId}/usersboards/${userId}`, {
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accessToken}`
      },
      method: 'PUT',
      body: JSON.stringify({
        userId: userId,
        boardId,
        permission: userPermission
      })
    }).then(updatedPermissionRes => {
      if (updatedPermissionRes.status >= 400) {
        throw new Error('Bad response from server.')
      }
      return updatedPermissionRes
    })
  }

  static _getUsersWS = function (accessToken) {
    return apiFetch(`users/`, {
      headers: {
        'Accept': 'application/json',
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accessToken}`
      },
      method: 'GET'
    }).then(usersRes => {
      if (usersRes.status >= 400) {
        throw new Error('Bad response from server. Check if Board Id is correct')
      }
      return usersRes.json()
    })
  }
}
