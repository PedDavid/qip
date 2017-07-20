import {SimplePoint} from './SimplePoint'
import {Rect} from './Rect'

// Figure: function (id, isClosedForm = false) {
export function Figure (figureStyle, id = null) {
  this.id = id
  this.subFigureLevel = 0   // subnivel da figura -> indica quantas subfiguras já gerou
  this.points = []
  // this.isClosedForm = isClosedForm;
  this.figureStyle = figureStyle

  this.getId = function () {
    return this.id.toString()
  }

  this.getTopLeftPoint = function () {
    let mostUpperLeftPointX = null
    let mostUpperLeftPointY = null
    this.points.forEach(point => {
      if (mostUpperLeftPointX == null || (point.x < mostUpperLeftPointX)) {
        mostUpperLeftPointX = point.x
      }
      if (mostUpperLeftPointY == null || (point.y < mostUpperLeftPointY)) {
        mostUpperLeftPointY = point.y
      }
    })
    return new SimplePoint(mostUpperLeftPointX, mostUpperLeftPointY)
  }

  this.getBottomRightPoint = function () {
    let mostBottomRightPointX = null
    let mostBottomRightPointY = null
    this.points.forEach(point => {
      if (mostBottomRightPointX == null || (point.x > mostBottomRightPointX)) {
        mostBottomRightPointX = point.x
      }
      if (mostBottomRightPointY == null || (point.y > mostBottomRightPointY)) {
        mostBottomRightPointY = point.y
      }
    })
    return new SimplePoint(mostBottomRightPointX, mostBottomRightPointY)
  }

  this.addPoint = function (point) {
    this.points.push(point) // new Point { x, y, this.id }
  }

  this.addPoints = function (points) {
    points.forEach(point => this.points.push(point))
  }

  this.getNearPoints = function (x, y, maxLinePart) {
    if (this.points.length === 1) {
      return this.points
    }
    return this.points.filter(point => {
      // TODO(PedDavid): Refactor
      if (point.x < x - maxLinePart * 0.70 || point.x > x + maxLinePart * 0.70) {
        return false
      }
      if (point.y < y - maxLinePart * 0.70 || point.y > y + maxLinePart * 0.70) {
        return false
      }
      return true
    })
  }

  this.removePoint = function (point) {
    const idxToRem = this.points.indexOf(point)
    const pts = this.points

    if (idxToRem < 0) {   // Point doesn't exist
      return []
    }

    if (idxToRem !== 0 && idxToRem !== pts.length) {  // se não for o primeiro nem o ultimo ponto da figura
      this.points = pts.slice(0, idxToRem)          // a figura atual fica com os pontos até o idx de remoção (não incluindo o ponto de remoção)
      this.subFigureLevel++                         // aumenta o subnivel de figura em que está

      const newFig = new Figure(this.figureStyle, newSubId(this.id))  // cria uma nova figura
      newFig.points = pts.slice(idxToRem)           // que fica com o resto dos pontos
      newFig.subFigureLevel = this.subFigureLevel   // o subnivel fica igual ao da sua figura mãe

      // Remove recursivamente outras ocorrencias do ponto na nova figura
      // Returnado o retorno da chamada, mais a figura gerada ao remover o ponto
      return [newFig, ...newFig.removePoint(point)]
    }

    // caso contrario
    this.points.splice(idxToRem, 1)                 // remover apenas o ponto
    return this.removePoint(point)                  // e tenta remover outras ocorrencias recursivamente, retornando o seu retorno
  }

  this.isEmpty = function () {
    return this.points.length === 0
  }

  this.draw = function (ctx, currScale) {
    ctx.strokeStyle = this.figureStyle.color
    ctx.save()
    // este scale tem em conta o estado em que a figura foi guardada.
    // Por exemplo se a figura 1 foi desenhada e foi feito zoom para o dobro esta figura é desenhada para o dobro.
    // Se outra figura for desenhada e, novamente, for feito zoom para o dobro, a primeira figura é desenhada com o quadruplo do zoom enquanto a segunda apenas é desenhada com o dobro do zoom
    ctx.scale((1 / this.figureStyle.scale) * currScale, (1 / this.figureStyle.scale) * currScale)

    let [prev, ...rest] = this.points

    // if the figure contains only 1 point, just draw the point. With the below algorithm, the point would not be drawed
    if (this.points.length === 1) {
      prev.draw(this.id, this.figureStyle.color, ctx)
    }

    rest.forEach(cur => {
      ctx.beginPath()
      ctx.moveTo(prev.x, prev.y)
      ctx.lineTo(cur.x, cur.y)
      ctx.lineWidth = cur.getStyleOf(this.id).width
      ctx.lineJoin = ctx.lineCap = 'round'
      ctx.stroke()

      prev = cur
    })

    ctx.restore()
  }

  // function hasPoint (point) {
  //  return this.points.indexOf(point) > -1
  // }

  function newSubId (id) {
    return id + 1 / Math.pow(2, this.subFigureLevel)
  }

  this.exportWS = function (boardId, extraFunc) {
    const toExportFig = this._export()
    toExportFig.tempId = toExportFig.id
    delete toExportFig.id

    if (extraFunc != null) {
      extraFunc(toExportFig)
    }

    const objToSend = {
      type: 'CREATE_LINE',
      owner: parseInt(boardId), // todo: retirar isto daqui
      payload: toExportFig
    }

    return JSON.stringify(objToSend)
  }

  this.exportLS = function () {
    return this._export()
  }

  this._export = function () {
    // map currentFigure's points to a data object
    const currentFigureTwin = Object.assign({}, this) // Object.assign() method only copies enumerable and own properties from a source object to a target object
    currentFigureTwin.points = this.points.map((point, idx) => {
      return new SimplePoint(point.x, point.y, point.getStyleOf(this.id), idx)
    })

    // map object so it can be parsed by api
    // todo: change model to join this
    currentFigureTwin.style = currentFigureTwin.figureStyle
    delete currentFigureTwin.figureStyle

    return currentFigureTwin
  }

  this.containsPoint = function (coord, event, grid, scale) {
    const canvasContext = event.target.getContext('2d')
    const points = this.getNearPoints(coord.x / scale, coord.y / scale, grid.getMaxLinePart())

    if (this.points == null) {
      return
    }

    let prev = points[0]
    for (let k = 0; k <= points.length; k++) {
      let currPoint = points[k]
      if (currPoint == null) {
        break
      }

      prev.x = prev.x * scale
      prev.y = prev.y * scale
      currPoint.x = currPoint.x * scale
      currPoint.y = currPoint.y * scale
      const width = currPoint.getStyleOf(this.id).width
      const rect = new Rect(prev, currPoint, width, canvasContext)

      const canvasWidth = canvasContext.canvas.width
      const canvasHeight = canvasContext.canvas.height

      // check if coord is inside line, adding some margin
      for (let c = (coord.x - 10) > 0 ? (coord.x - 10) : 0; c < canvasWidth && c < coord.x + 10; ++c) {
        for (let l = (coord.y - 10) > 0 ? (coord.y - 10) : 0; l < canvasHeight && l < (coord.y + 10); ++l) {
          if (rect.contains({ x: c, y: l })) {
            return true
          }
        }
      }
      prev = currPoint
    }
    return false
  }

  this.scale = function (scaleType, offsetPoint) {

  }
}

export function FigureStyle (color, scale) {
  this.color = color
  this.scale = scale
}
