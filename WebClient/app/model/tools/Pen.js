import { Figure, FigureStyle } from './../Figure'
import {SimplePoint} from './../SimplePoint'
import {PointStyle} from './../Point'
import Tool from './Tool'
import SettingsConfig from './../../util/SettingsConfig.js'

export default class Pen implements Tool {
  constructor (grid, color, width) {
    this.type = 'pen'
    this.color = color
    this.width = width
    this.fontText = width

    this.grid = grid
    this.currentFigure = null
  }

  // passar para aqui as funções getMousePos
  onPress (event, scale, updateCanvasSizeFunc, settings) {
    if (settings[SettingsConfig.fingerMoveSettingIdx] && event.pointerType === 'touch') {
      return
    }
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

    updateCanvasSizeFunc(x, y)
  }

  onSwipe (event, scale, updateCanvasSizeFunc, settings) {
    if (event.buttons > 0 && !(settings[SettingsConfig.fingerMoveSettingIdx] && event.pointerType === 'touch')) {
      const x = event.offsetX
      const y = event.offsetY

      if (this.currentFigure === null) {
        return
      }

      const press = event.pressure * this.width
      const pointStyle = new PointStyle(press)
      this.currentFigure.points.length === 0 &&
        this.currentFigure.addPoint(new SimplePoint(x, y, pointStyle)) // this is necessary to avoid a bug when user draws out of the screen
      const last = this.currentFigure.points[this.currentFigure.points.length - 1]
      // Ignore if swiped to last coordinates
      if (last.x === x && last.y === y) {
        return
      }

      this.currentFigure.addPoint(new SimplePoint(x, y, pointStyle))

      const canvasContext = event.target.getContext('2d')
      canvasContext.beginPath() // também tem de estar aqui para dar para fazer pressure sensitive

      canvasContext.moveTo(last.x, last.y)
      canvasContext.lineTo(x, y)

      canvasContext.lineWidth = press
      canvasContext.lineJoin = canvasContext.lineCap = 'round'
      canvasContext.stroke()

      updateCanvasSizeFunc(x, y)
    }
  }

  onPressUp (event, persist) {
    if (this.currentFigure === null) { // it is necessary to avoid some minor bugs
      return
    }
    // todo: passar persistencia para o onOut
    this.grid.addFigure(this.currentFigure, true)

    persist.sendPenAction(this.currentFigure, this.grid.getCurrentFigureId())

    // reset current figure
    this.currentFigure = null
  }

  onOut (event, persist) {
    if (this.currentFigure === null || this.currentFigure.points.length <= 0) {
      return
    }
    this.grid.addFigure(this.currentFigure, true)

    persist.sendPenAction(this.currentFigure, this.grid.getCurrentFigureId())

    // fazer reset à figura para que não continue a desenhar se o utilizador sair da área do canvas
    // Desta forma, se o utilizador voltar à área do canvas com o ponteiro premido, irá desenhar uma nova figura
    const figStyle = new FigureStyle(this.color)
    this.currentFigure = new Figure(figStyle)
  }

  onContextMenu (event) {}

  equals (pen) {
    return pen instanceof Pen && this.width === pen.width && this.color === pen.color
  }
}
