import {Figure, FigureStyle} from './../model/Figure'
import {PointStyle} from './../model/Point'
import {SimplePoint} from './../model/SimplePoint'
import fetch from 'isomorphic-fetch'
import Grid from './../model/Grid'
import Pen from './../model/tools/Pen'
import Eraser from './../model/tools/Eraser'
import Move from './../model/tools/Move'

export class Persist {
  // todo: check if it's necessary all parameters
  constructor (persistType: PersistType, canvasContext, grid) {
    this.persistType = persistType
    this.canvasContext = canvasContext
    this.socket = null
    this.grid = grid
    this.connected = false
    this.boardId = null
  }

  connect = function (boardId) {
    // TODO(peddavid): Configuration of endpoints
    const scheme = document.location.protocol === 'https:' ? 'wss' : 'ws'
    const port = document.location.port ? (57059) : ''
    const connectionUrl = `${scheme}://localhost:${port}/ws/${boardId}`
    console.log('making web socket connection to: ' + connectionUrl)
    this.socket = new WebSocket(connectionUrl)

    this.socket.onopen = (event) => {
      console.info('web socket connection to: ' + connectionUrl + ' is now open')
      this._configureWSProtocol()
      this.connected = true
      this.boardId = boardId
      this.persistType = PersistType().WebSockets
      this._persistBoardByWS()
    }

    this.socket.onerror = (event) => {
      console.error('an error occurred on web socket connection to: ' + connectionUrl)
      this.connected = false
      this.boardId = null
    }
  }

  _configureWSProtocol = function () {
    this.socket.onmessage = (event) => {
      const {type, payload} = JSON.parse(event.data)
      switch (type) {
        case 'CREATE_LINE':
          // update maxFigureId
          // this.grid.updateCurrentFigIdIfGreater(payload.id)
          if (payload.tempId != null) {
            // update figure
            const prevFigure = this.grid.getFigure(payload.tempId)
            // update point's figures of updated figures
            prevFigure.points.forEach(point => {
              const pointStyle = point.getStyleOf(payload.tempId)
              point.removeFigure(payload.tempId)
              point.addFigure(payload.id, pointStyle)
            })
            // update figure id
            prevFigure.id = payload.id

            // update grid map of figures
            this.grid.updateMapFigure(payload.tempId, prevFigure)

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
          this.grid.addFigure(newFigure)
          this.grid.draw(this.canvasContext, 1)
          console.log('received new line with id ' + payload.figure.Id)
          break
        case 'DELETE_LINE':
          const figureToDelete = this.grid.getFigure(payload.figure.Id)
          this.grid.removeFigure(figureToDelete, this.canvasContext, 1)
          break
        case 'ALTER_LINE':
          // todo: por estes comentários em vez de apagar e criar a figura quando o servidor estiver a enviar o offsetPoint
          // const figureToMove = this.grid.getFigure(payload.id)
          // this.grid.moveFigure(figureToMove, payload.offsetPoint, this.canvasContext, 1)
          const figureToMove = this.grid.getFigure(payload.id)
          this.grid.removeFigure(figureToMove, this.canvasContext, 1)
          figureToMove.points = payload.points
          this.grid.addFigure(figureToMove)
          this.grid.draw(this.canvasContext, 1)
          break
      }
    }
  }

  getInitialBoardAsync = function (boardId) {
    if (this.persistType === PersistType().WebSockets) {
      return this._getInitialBoardWS(boardId)
    } else if (this.persistType === PersistType().LocalStorage) {
      return this._getInitialBoardLS()
    }
  }

  // get initial board from server by web sockets
  _getInitialBoardWS = function (boardId) {
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
      }),
      fetch(`http://localhost:57059/api/boards/${boardId}/figures/images`, {
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
      const lines = figures
      let maxId = -1
      console.log('lines fetched from initial board:')
      console.log(figures)
      // todo: make possible to get images too
      return new Grid(lines, maxId)
      // const pen = new Pen(this.grid, 'black', 5)
      // const eraser = new Eraser(this.grid, 5)

      // update current board id and connect to websockets
      // this.updateBoardId(boardId)
    })
  }

  // get initial board from local storage
  _getInitialBoardLS = function () {
    console.info('getting board data from local storage')
    return new Promise((resolve, reject) => {
      // if it's not authenticated or not sharing board, get data from localstorage
      if (window.localStorage.getItem('figures') === null && window.localStorage.getItem('defaultPen') === null && window.localStorage.getItem('defaultEraser') === null) {
        this._resetLocalStorage()
      }

      const figures = JSON.parse(window.localStorage.getItem('figures'))
      const nextFigureId = JSON.parse(window.localStorage.getItem('currFigureId'))
      const grid = new Grid(figures, nextFigureId)
      const tempPen = JSON.parse(window.localStorage.getItem('defaultPen'))
      const tempEraser = JSON.parse(window.localStorage.getItem('defaultEraser'))
      const tempCurrTool = JSON.parse(window.localStorage.getItem('currTool'))
      const currTool = this._getToolFromLS(grid, tempCurrTool)
      const favorites = JSON.parse(window.localStorage.getItem('favorites'))
        .map(fav => this._getToolFromLS(grid, fav))

      const initBoard = {
        grid,
        defaultPen: new Pen(grid, tempPen.color, tempPen.width),
        defaultEraser: new Eraser(grid, tempEraser.width),
        currTool,
        favorites
      }

      resolve(initBoard)
    })
  }

  _getToolFromLS = function (grid, rawTool) {
    switch (rawTool.type){
      case ('pen') : 
        return new Pen(grid, rawTool.color, rawTool.width)
        break
      case ('eraser') :
        return new Eraser (grid, rawTool.width)
        break
      case ('move'):
        return new Move(grid)
    }
  }

  _resetLocalStorage = function () {
    const tempGrid = new Grid([], -1)
    window.localStorage.setItem('figures', JSON.stringify(tempGrid.getFiguresArray()))
    window.localStorage.setItem('currFigureId', JSON.stringify(tempGrid.getCurrentFigureId()))
    window.localStorage.setItem('defaultPen', JSON.stringify(new Pen(tempGrid, 'black', 5)))
    window.localStorage.setItem('defaultEraser', JSON.stringify(new Eraser(tempGrid, 20)))
    window.localStorage.setItem('favorites', '[]')
    window.localStorage.setItem('currTool', JSON.stringify(new Pen(tempGrid, 'black', 5)))
  }

  _persistBoardByWS = function () {
    if (this.connected) {
      const figures = JSON.parse(window.localStorage.getItem('figures'))
      figures.forEach(fig => {
        fig.tempId = fig.id
        delete fig.id
        const objToSend = {
          type: fig.type === 'figure' ? 'CREATE_LINE' : 'CREATE_IMAGE',
          payload: fig
        }
        this.socket.send(JSON.stringify(objToSend))
      })
    }

    this._resetLocalStorage()
  }

  cleanCanvas = function () {
    if (this.persistType === PersistType().WebSockets) {
      return this._cleanCanvasWS()
    } else if (this.persistType === PersistType().LocalStorage) {
      return this._resetLocalStorage()
    }
  }

  _cleanCanvasWS = function () {
    // todo: add an action in server to clean board
    this.grid.getFiguresArray().forEach(fig => {
      if (this.connected) {
        const objToSend = {
          type: 'DELETE_LINE',
          payload: {'id': fig.id}
        }
        this.socket.send(JSON.stringify(objToSend))
      }
    })
  }
}

export function PersistType () {
  return {
    WebSockets: 0,
    LocalStorage: 1
  }
}
