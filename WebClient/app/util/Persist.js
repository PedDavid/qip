// @Flow

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
  }

  connect = function (boardId) {
    const scheme = document.location.protocol === 'https:' ? 'wss' : 'ws'
    const port = document.location.port ? (53379) : ''
    const connectionUrl = `${scheme}://${document.location.hostname}:${port}/api/websockets?id=${this.boardId}`
    this.socket = new WebSocket(connectionUrl)

    this.socket.onopen = (event) => {
      this._configureWSProtocol()
      this.connected = true
    }
  }

  _configureWSProtocol = function () {
    this.socket.onmessage = (event) => {
      const {type, payload} = JSON.parse(event.data)
      switch (type) {
        case 'INSERT_FIGURE':
          const figStyle = new FigureStyle(payload.figureStyle.color, payload.figureStyle.scale)
          const newFigure = new Figure(figStyle, payload.id)
          newFigure.points = payload.points
          this.addFigure(newFigure)
          this.grid.draw(this._canvasContext, 1)
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
    // fetch data of current board
    return Promise.all([
      fetch(`http://localhost:57251/api/boards/${boardId}/figures/lines`, {
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        },
        method: 'GET'
      }),
      fetch(`http://localhost:57251/api/boards/${boardId}/figures/images`, {
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
      return {lines: responses[0].json(), images: responses[1].json()}
    }).then(figures => {
      const ids = figures.map(figure => figure.map(innerFig => innerFig.id))
      var max = ids.reduce((a, b) => {
        return Math.max(a, b)
      })

      // todo: make possible to get images too
      return new Grid(figures.lines, max)
      // const pen = new Pen(this.grid, 'black', 5)
      // const eraser = new Eraser(this.grid, 5)

      // update current board id and connect to websockets
      // this.updateBoardId(boardId)
    })
  }

  // get initial board from local storage
  _getInitialBoardLS = function () {
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
      const grid = new Grid(figures, currFigureId)
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
