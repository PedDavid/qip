// Figure: function (id, isClosedForm = false) {
export function Figure (id, isClosedForm, figureStyle) {
  this.id = id
  this.subFigureLevel = 0   // subnivel da figura -> indica quantas subfiguras já gerou
  this.points = []
  // this.isClosedForm = isClosedForm;
  this.figureStyle = figureStyle

  this.getId = function () {
    return this.id.toString()
  }

  this.addPoint = function (point) {
    this.points.push(point)
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

      const newFig = new Figure(newSubId(this.id))  // cria uma nova figura
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
}

export function FigureStyle (color, scale) {
  this.color = color
  this.scale = scale
}
