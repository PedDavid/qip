import {Figure, FigureStyle} from './../../model/Figure'
import {PointStyle} from './../../model/Point'
import {SimplePoint} from './../../model/SimplePoint'
import fetch from 'isomorphic-fetch'
import Grid from './../../model/Grid'
import BoardData from './../../model/BoardData'

export default class PersistLS {
  static _configureWSProtocol = function (socket, grid, canvasContext) {
    socket.onmessage = (event) => {
      const {type, payload} = JSON.parse(event.data)
      switch (type) {
        case 'CREATE_LINE':
          // update maxFigureId
          // grid.updateCurrentFigIdIfGreater(payload.id)
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
          const figureToDelete = grid.getFigure(payload.figure.Id)
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
  static _persistBoardByWS = function (socket) {
    const figures = JSON.parse(window.localStorage.getItem('figures'))
    figures.forEach(fig => {
      fig.tempId = fig.id
      delete fig.id
      const objToSend = {
        type: fig.type === 'figure' ? 'CREATE_LINE' : 'CREATE_IMAGE',
        payload: fig
      }
      socket.send(JSON.stringify(objToSend))
    })
  }

  // get initial board from server by web sockets
  static _getInitialBoardWS = function (boardId) {
    // todo: fazer também um fetch ao board específico e obter o maxFigureId em vez de estar a verificar qual o maior atrvés das figuras recebidas
    console.info('getting board data from server by web sockets')
    // fetch data of current board
    return Promise.all([
      fetch(`http://localhost:57059/api/boards/${boardId}/figures/lines`, {
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
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
          'Content-Type': 'application/json'
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
      console.log('lines fetched from initial board:')
      console.log(figures)
      // todo: make possible to get images too
      const grid = new Grid(figures, maxId)

      const initBoard = {
        grid
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

  static _updateFavoritesWS = function (newFavs) {}

  static _updateCurrToolWS = function (newCurrTool) {}

  static _updateCanvasSizeWS = function (canvasSize) {}

  // get user info from server by web sockets
  static _getUserInfoAsyncWS = function (grid, profile) {
    console.info('getting user info from server by web sockets')
    // fetch data of current board
    return Promise.all([
      fetch(`http://localhost:57059/api/users/0`, { // todo: alterar id do user
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        },
        method: 'GET'
      })
    ]).then(responses => {
      if (responses.some(res => res.status >= 400)) {
        throw new Error('Bad response from server. Check if Board Id is correct')
      }
      return responses[0].json()
    }).then(figures => {
      const userInfo = {
        defaultPen: null,
        defaultEraser: null,
        currTool: null,
        favorites: null,
        userBoards: null,
        settings: null
      }
      return userInfo
    })
  }

  // get board info from server by web sockets
  static _getBoardInfoWS = function (boardId) {
    console.info('getting board info from server by web sockets')
    // fetch data of board
    return Promise.all([
      fetch(`http://localhost:57059/api/boards/${boardId}`, {
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        },
        method: 'GET'
      })
    ]).then(responses => {
      if (responses.some(res => res.status >= 400)) {
        throw new Error('Bad response from server. Check if Board Id is correct')
      }
      return responses[0].json()
    }).then(board => {
      return new BoardData(board.id, board.name)
    })
  }
}
