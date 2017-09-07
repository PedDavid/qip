import { SimplePoint } from './../SimplePoint'
import { Figure } from './../Figure'
import Line from './../Line'
import { Image } from './../Image'
import Selection from './../Selection'

import Tool from './Tool'

export default class Move implements Tool {
  constructor (grid) {
    this.type = 'move'
    this.grid = grid
    this.movingLine = null
    this.currentFigureMoving = null
    this.scaling = false
  }

  // passar para aqui as funções getMousePos
  onPress (event, scale) { // current
    const x = event.offsetX
    const y = event.offsetY

    // get line figures
    if (this.currentFigureMoving === null) {
      const figures = this.grid.getNearestFigures(x / scale, y / scale)
        .concat(this.grid.getImagesArray())
        .sort((fig1, fig2) => Math.abs(fig2.id) - Math.abs(fig1.id)) // inverse order to remove the newest first
        .filter(lin => lin.containsPoint(new SimplePoint(x, y), event, this.grid, scale))

      this.currentFigureMoving = figures[0]

      // make selection here so it does not repeat
      if (this.currentFigureMoving != null) {
        const canvasContext = event.target.getContext('2d')
        this.selection = new Selection(this.currentFigureMoving.getTopLeftPoint(), this.currentFigureMoving.getBottomRightPoint(), canvasContext)
        this.selection.select(event.pointerType === 'touch')
      }
    }

    // check if user tap in the scaling circles
    if (this.currentFigureMoving != null) {
      this.scaling = this.selection.isScaling(new SimplePoint(x, y), event.pointerType === 'touch')
    }

    if (this.selection != null && this.selection.containsPoint(new SimplePoint(x, y))) {
      const point = new SimplePoint(x, y)
      this.movingLine = new Line(point, point)
    } else {
      this.currentFigureMoving = null
      this.selection = null
      const canvasContext = event.target.getContext('2d')
      this.grid.draw(canvasContext, 1) // call draw to reset selection rect
    }
  }

  onSwipe (event, scale) {
    if (event.buttons <= 0 || this.movingLine === null) {
      return
    }
    const x = event.offsetX
    const y = event.offsetY
    const canvasContext = event.target.getContext('2d')

    if (this.scaling !== false) {
      const offsetPoint = new SimplePoint(
        x - this.movingLine.end.x,
        y - this.movingLine.end.y)
      this.currentFigureMoving.scale(this.scaling, offsetPoint, this.grid, canvasContext)
      this.grid.draw(canvasContext, 1)
    } else if (this.movingLine != null && this.currentFigureMoving instanceof Image) {
      const srcPoint = this.currentFigureMoving.getSrcPoint()
      const destPoint = new SimplePoint(
        srcPoint.x + (x - this.movingLine.end.x),
        srcPoint.y + (y - this.movingLine.end.y))
      this.grid.moveImage(this.currentFigureMoving, destPoint, canvasContext, scale)
    } else if (this.movingLine != null && this.currentFigureMoving instanceof Figure) {
      const offsetPoint = new SimplePoint(x - this.movingLine.end.x, y - this.movingLine.end.y)
      this.grid.moveLine(this.currentFigureMoving, point => {
        return this.grid.getOrCreatePoint(point.x + offsetPoint.x, point.y + offsetPoint.y)
      }, canvasContext, scale)
    }
    this.movingLine = new Line(this.movingLine.start, {x, y}) // always preserve start point and update final point

    if (this.currentFigureMoving != null) {
      this.selection.move(this.currentFigureMoving.getTopLeftPoint(), this.currentFigureMoving.getBottomRightPoint())
      this.selection.select(event.pointerType === 'touch')
    }
  }

  onPressUp (event, persist) {
    this.onOut(event, persist)
  }

  onOut (event, persist) {
    if (this.movingLine != null && this.movingLine.start !== this.movingLine.end) {
      if (persist.connected) {
        const offsetPoint = new SimplePoint(this.movingLine.end.x - this.movingLine.start.x, this.movingLine.end.y - this.movingLine.start.y)
        const toSend = this.currentFigure.exportWS(
          fig => { fig.offsetPoint = offsetPoint }
        )
        persist.socket.send(toSend)
      } else {
        // move from localstorage
        const dataFigure = JSON.parse(window.localStorage.getItem('figures'))
        const toPersist = this.currentFigureMoving.exportLS()
        let figIdx = dataFigure.findIndex(f => f.id === toPersist.id)
        dataFigure[figIdx] = toPersist
        window.localStorage.setItem('figures', JSON.stringify(dataFigure))
      }
    }

    this.movingLine = null
    this.scaling = false
  }

  equals (move) {
    return move instanceof Move
  }
}
