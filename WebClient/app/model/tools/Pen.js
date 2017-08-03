import { Figure, FigureStyle } from './../Figure'
import {SimplePoint} from './../SimplePoint'
import {PointStyle} from './../Point'
import Tool from './Tool'

export default class Pen implements Tool {
  constructor (grid, color, width) {
    this.type = 'pen'
    this.color = color
    this.width = width

    this.grid = grid
    this.currentFigure = null
  }

  // passar para aqui as funções getMousePos
  onPress (event, scale) {
    const x = event.offsetX
    const y = event.offsetY

    const figStyle = new FigureStyle(this.color, scale)
    // Create a new Figure
    this.currentFigure = new Figure(figStyle)

    const press = event.pressure * this.width
    const pointStyle = new PointStyle(press)
    this.currentFigure.addPoint(new SimplePoint(x, y, pointStyle))

    const canvasContext = event.target.getContext('2d')
    canvasContext.beginPath()
    canvasContext.arc(x, y, press / 4, 0, 2 * Math.PI, false)
    canvasContext.fillStyle = this.color
    canvasContext.fill()
    canvasContext.lineWidth = press / 2
    canvasContext.strokeStyle = this.color
    canvasContext.stroke()
  }

  onSwipe (event, scale) {
    if (event.buttons > 0) {
      const x = event.offsetX
      const y = event.offsetY

      if (this.currentFigure === null) {
        return
      }

      const press = event.pressure * this.width
      const last = this.currentFigure.points[this.currentFigure.points.length - 1]
      // Ignore if swiped to last coordinates
      if (last.x === x && last.y === y) {
        return
      }

      const pointStyle = new PointStyle(press)
      this.currentFigure.addPoint(new SimplePoint(x, y, pointStyle))

      const canvasContext = event.target.getContext('2d')
      canvasContext.beginPath() // também tem de estar aqui para dar para fazer pressure sensitive

      canvasContext.moveTo(last.x, last.y)
      canvasContext.lineTo(x, y)

      canvasContext.lineWidth = press
      canvasContext.lineJoin = canvasContext.lineCap = 'round'
      canvasContext.stroke()
    }
  }

  onPressUp (event, persist) {
    // todo: passar persistencia para o onOut
    this.grid.addFigure(this.currentFigure)

    if (persist.connected) {
      persist.socket.send(this.currentFigure.exportWS())
    } else {
      // add to localstorage
      const dataFigure = JSON.parse(window.localStorage.getItem('figures'))
      dataFigure.push(this.currentFigure.exportLS()) // it can be push instead of dataFigure[id] because it will not have crashes with external id's because it's only used when there is no connection
      window.localStorage.setItem('figures', JSON.stringify(dataFigure))
      window.localStorage.setItem('currFigureId', JSON.stringify(this.grid.getCurrentFigureId()))
    }

    // reset current figure
    this.currentFigure = null
  }

  onOut (event, socket) {
    const grid = this.grid
    if (this.currentFigure === null) {
      return
    }
    grid.addFigure(this.currentFigure)

    // fazer reset à figura para que não continue a desenhar se o utilizador sair da área do canvas
    // Desta forma, se o utilizador voltar à área do canvas com o ponteiro premido, irá desenhar uma nova figura
    const figStyle = new FigureStyle(this.color)
    this.currentFigure = new Figure(figStyle)
  }

  equals (pen) {
    return pen instanceof Pen && this.width === pen.width && this.color === pen.color
  }
}
