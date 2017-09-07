import {Figure, FigureStyle} from './../../model/Figure'
import {PointStyle} from './../../model/Point'
import {SimplePoint} from './../../model/SimplePoint'
import fetch from 'isomorphic-fetch'
import Grid from './../../model/Grid'
import Pen from './../../model/tools/Pen'
import Eraser from './../../model/tools/Eraser'
import Move from './../../model/tools/Move'
import BoardData from './../../model/BoardData'

export default class PersistLS {
  static _configureWSProtocol = function (socket, grid, canvasContext) {
    socket.onmessage = (event) => {
      const {type, payload} = JSON.parse(event.data)
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
            // update figure id
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
          // const figureToMove = grid.getFigure(payload.id)
          // grid.moveFigure(figureToMove, payload.offsetPoint, canvasContext, 1)
          const figureToMove = grid.getFigure(payload.id)
          grid.removeFigure(figureToMove, canvasContext, 1)
          figureToMove.points = payload.points
          grid.addFigure(figureToMove)
          grid.draw(canvasContext, 1)
          break
      }
    }
  }

  // todo: enviar também os favoritos, current tool e prev tools
  // this procedure sends to server all data stored in local storage.
  // it should be called when user authenticates but there was already some information in local storage
  static _persistBoardByWS = function (socket) {
    const figures = JSON.parse(window.localStorage.getItem('figures'))
    figures.forEach(fig => {
      fig.tempId = fig.id
      delete fig.id
      fig.persistLocalBoard = true
      const objToSend = {
        type: fig.type === 'figure' ? 'CREATE_LINE' : 'CREATE_IMAGE',
        payload: fig
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
      fetch(`http://localhost:57059/api/boards/${boardId}/figures/lines`, {
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
      fetch(`http://localhost:57059/api/boards/${boardId}/figures/images`, {
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
      const figures = allFigs[0].concat(allFigs[1])
      let maxId = -1
      console.log('figures fetched from initial board:')
      console.log(figures)
      // todo: make possible to get images too
      const grid = new Grid(figures, maxId)

      const initBoard = {
        grid,
        canvasSize: { // todo: get from server
          width: 0,
          height: 0
        }
      }

      return initBoard
    })
  }

  static _cleanCanvasWS = function (grid, socket) {
    // todo: add an action in server to clean board
    grid.getFiguresArray().forEach(fig => {
      const objToSend = {
        type: 'DELETE_LINE',
        payload: {'id': fig.id}
      }
      socket.send(JSON.stringify(objToSend))
    })
  }

  static _updateCanvasSizeWS = function (canvasSize) {}

  static _updateUserPreferencesWS = function (updatedPreferences, profile, accessToken) {
    // preferences has
    // favorites | pen colors | defaultPen
    // DefaultEraser | CurrTool | Settings

    return fetch(`http://localhost:57059/api/users/${profile.sub}/preferences`, {
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
      return updatedPreferencesRes.json()
    }).then(updatedPreferences => {
      console.log(updatedPreferences)
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
    return fetch(`http://localhost:57059/api/boards/`, {
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
      fetch(`http://localhost:57059/api/users/${profile.sub}/preferences`, {
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
        return preferencesRes.json()
      }),
      fetch(`http://localhost:57059/api/users/${profile.sub}/boards`, {
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
    switch (rawTool.type) {
      case ('pen'):
        return new Pen(grid, rawTool.color, rawTool.width)
      case ('eraser'):
        return new Eraser(grid, rawTool.width)
      case ('move'):
        return new Move(grid)
    }
  }

  // get board info from server by web sockets
  static _getBoardInfoWS = function (boardId, profile, accessToken) {
    console.info('getting board info from server by web sockets')
    // if user is not authenticated, user permission is public permission so there is no need to do second fetch
    const promises = [
      fetch(`http://localhost:57059/api/boards/${boardId}`, {
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
        fetch(`http://localhost:57059/api/users/${profile.sub}/boards/${boardId}`, {
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
        const userBoardInfo = allRes.length === 1 ? boardInfo.basePermission : allRes[1] // if user is not authenticated, board permissions is 0
        return new BoardData(boardInfo.id, boardInfo.name, boardInfo.basePermission, userBoardInfo.permission)
      })
  }
}
