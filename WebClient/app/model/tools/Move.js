import { SimplePoint } from './../SimplePoint'
import { Figure, FigureStyle } from './../Figure'
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

    this.tryCloseContextMenu()

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
    const moveType = this.currentFigureMoving instanceof Figure ? 'figure' : 'image'
    if (this.movingLine != null && this.movingLine.start !== this.movingLine.end) {
      const offsetPoint = new SimplePoint(this.movingLine.end.x - this.movingLine.start.x, this.movingLine.end.y - this.movingLine.start.y)
      persist.sendMoveAction(this.currentFigureMoving, offsetPoint, this.scaling, moveType)
    }

    this.movingLine = null
    this.scaling = false
  }

  onContextMenu (contextMenuEvent, persist, addContextMenuFunc, closeContextMenu, canvasContext) {
    contextMenuEvent.preventDefault()
    contextMenuEvent.persist() // must be persisted to not be reused. therefore, when onClick of menu items are triggered, this event is valid

    this.onCloseContextMenu = closeContextMenu
    let contextMenuRaw
    const copyMenuItem = {
      icon: 'copy',
      text: 'Copy',
      onClick: () => {
        const toSave = this.currentFigureMoving.exportLS()
        delete toSave.id
        persist.addClipboard(toSave)
        this.tryCloseContextMenu()
      }}

    const pasteMenuItem = {
      icon: 'paste',
      text: 'Paste',
      onClick: (event) => {
        const clipboardFig = persist.getClipboard()
        this._pasteFigure(contextMenuEvent, clipboardFig.type, clipboardFig, persist, canvasContext)
        this.tryCloseContextMenu()
      }}

    this.currentFigureMoving instanceof Image && (contextMenuRaw = [{
      header: {icon: null, text: 'Edit'},
      menuItems: [{
        icon: 'trash',
        text: 'Remove',
        onClick: () => {
          this.grid.removeImage(this.currentFigureMoving.id, canvasContext, 1)
          persist.removeImage(this.currentFigureMoving.id)
          this.tryCloseContextMenu()
          this.currentFigureMoving = null
        }},
        copyMenuItem, pasteMenuItem]
    }])

    this.currentFigureMoving instanceof Figure && (contextMenuRaw = [{
      header: {icon: null, text: 'Edit'},
      menuItems: [copyMenuItem, pasteMenuItem]
    }])

    this.currentFigureMoving === null && (contextMenuRaw = [{
      header: {icon: null, text: 'Edit'},
      menuItems: [pasteMenuItem]
    }])

    addContextMenuFunc(contextMenuEvent.clientX, contextMenuEvent.clientY, contextMenuRaw)
  }

  tryCloseContextMenu () {
    if (this.onCloseContextMenu != null) {
      this.onCloseContextMenu()
    }
  }

  _pasteFigure (event, type, figure, persist, canvasContext) {
    if (type === 'figure') {
      const newFig = new Figure(new FigureStyle(figure.style.color, 1))
      newFig.points = figure.points
      const copiedFigure = this.grid.addFigure(newFig, true)
      // move figure to the point user pressed
      const offsetPoint = new SimplePoint(event.clientX - copiedFigure.points[0].x, event.clientY - copiedFigure.points[0].y)
      this.grid.moveLine(copiedFigure, point => {
        return this.grid.getOrCreatePoint(point.x + offsetPoint.x, point.y + offsetPoint.y)
      }, canvasContext, 1)
      const movedFigure = this.grid.getFigure(copiedFigure.id) // send figure after it was moved
      persist.sendPenAction(movedFigure, this.grid.getCurrentFigureId())
      this.tryCloseContextMenu()
    } else if (type === 'image') {
      const newImage = new Image({x: event.clientX, y: event.clientY}, figure.Src, figure.Width, figure.Height, null, () => this.grid.draw(canvasContext, 1))
      // do not change this order. image must be added to grid first to set the new id
      this.grid.addImage(newImage)
      newImage.persist(persist, this.grid)
    }
    this.grid.draw(canvasContext, 1)
  }

  equals (move) {
    return move instanceof Move
  }
}
