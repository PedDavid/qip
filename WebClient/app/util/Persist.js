import {Figure, FigureStyle} from './../model/Figure'
import fetch from 'isomorphic-fetch'
import Grid from './../model/Grid'
import Pen from './../model/tools/Pen'
import Eraser from './../model/tools/Eraser'

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
    const connectionUrl = `${scheme}://localhost:${port}/api/websockets?id=${boardId}`
    console.log('making web socket connection to: ' + connectionUrl)
    this.socket = new WebSocket(connectionUrl)

    this.socket.onopen = (event) => {
      console.info('web socket connection to: ' + connectionUrl + ' is now open')
      this._configureWSProtocol()
      this.connected = true
      this.boardId = boardId
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
          this.grid.updateCurrentFigIdIfGreater(payload.id)
          if (payload.tempId != null) {
            // update figure id
            this.grid.getFigure(payload.tempId).id = payload.id
            console.log('updated line with id ' + payload.tempId + ' to id ' + payload.id)
            return
          }
          const figStyle = new FigureStyle(payload.style.color, 1)
          const newFigure = new Figure(figStyle, payload.id)
          newFigure.points = payload.points
          this.grid.addFigure(newFigure)
          this.grid.draw(this.canvasContext, 1)
          console.log('received new line with id ' + payload.id)
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
      let maxId = 5
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
      if (window.localStorage.getItem('figures') === null && window.localStorage.getItem('pen') === null && window.localStorage.getItem('eraser') === null) {
        const tempGrid = new Grid([], -1)
        window.localStorage.setItem('figures', JSON.stringify(tempGrid.getFigures()))
        window.localStorage.setItem('currFigureId', JSON.stringify(tempGrid.getCurrentFigureId()))
        window.localStorage.setItem('pen', JSON.stringify(new Pen(tempGrid, 'black', 5)))
        window.localStorage.setItem('eraser', JSON.stringify(new Eraser(tempGrid, 20)))
      }

      const figures = JSON.parse(window.localStorage.getItem('figures'))
      const nextFigureId = JSON.parse(window.localStorage.getItem('currFigureId')) + 1
      const grid = new Grid(figures, nextFigureId)
      // const tempPen = JSON.parse(window.localStorage.getItem('pen'))
      // const pen = new Pen(grid, tempPen.color, tempPen.width)
      // const tempEraser = JSON.parse(window.localStorage.getItem('eraser'))
      // const eraser = new Eraser(grid, tempEraser.width)

      resolve(grid)
    })
  }
}

export function PersistType () {
  return {
    WebSockets: 0,
    LocalStorage: 1
  }
}
