// TODO(simaovii): este pointStyle não pode estar aqui. adicioná-lo só quando se grava o ponto na figura
export function Point (x, y) {
  this.x = x
  this.y = y

  const figures = new Map() // map(key = figure ids, value = point style)

  this.getX = function () {
    return x
  }

  this.getY = function () {
    return y
  }

  this.addFigure = function (figureId, pointStyle) {
    if (!figures.has(figureId)) {
      figures.set(figureId, pointStyle)
      return true
    }
    return false
  }

  this.removeFigure = function (figureId) {
    figures.delete(figureId)
  }

  this.getFigureIds = function () {
    return Array.from(figures.keys())
  }

  this.getStyleOf = function (figureId) {
    return figures.get(figureId)
  }

  this.hasFigure = function (figureId) {
    return figures.has(figureId)
  }

  this.exports = function () {
    return {
      x: this.x,
      y: this.y,
      figures: Array.from(figures.entries())
    }
  }

  this.draw = function (figureId, color, ctx) {
    const press = this.getStyleOf(figureId).width
    ctx.beginPath()
    ctx.arc(this.x, this.y, press / 4, 0, 2 * Math.PI, false)
    ctx.fillStyle = color
    ctx.fill()
    ctx.lineWidth = press / 2
    ctx.stroke()
  }
}

export function PointStyle (width) {
  this.width = width
}
