// @Flow

import {Figure, FigureStyle} from './../model/Figure'

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
}

export function PersistType () {
  return {
    WebSockets: 0,
    LocalStorage: 1
  }
}
