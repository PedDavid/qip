export function Rect (lineP1, lineP2, width, canvasContext) {
  const getRectInitPoint = function () {
    // trocar os pontos caso o ponto inicial esteja mais à esquerda ou, caso a reta seja vertical, mais a cima
    // Isto serve para que o rectangulo a ser feito tenha ponto inicial fixo
    if (lineP2.x < lineP1.x || (lineP2.x === lineP1.x && lineP2.y < lineP1.y)) {
      const aux = lineP1
      lineP1 = lineP2
      lineP2 = aux
    }
    // Calcular o rectângulo. Apenas é necessário calcular o ponto inicial pois já se tem o comprimento e largura da reta
    const lineWidth = width
    // line equation => y = mx+n (we don't need n because we only want to calculate perpendicular line)
    // m = /(   a   ) / (   b   )
    // m = /(Yb - Ya) / (Xb - Xa)
    const a = lineP2.y - lineP1.y
    const b = (lineP2.x - lineP1.x)
    if (b !== 0 && a !== 0) { // not vertical line or not horizontal line
      const m = a / b
      const perpLineM = -1 / m

      const aux = Math.sqrt(1 + Math.pow(perpLineM, 2))
      const p1n1X = (lineWidth / 2) / aux + lineP1.x
      const p1n1Y = (perpLineM * (lineWidth / 2)) / aux + lineP1.y

      if (m < 0) {
        return {
          x: -(lineWidth / 2) / aux + lineP1.x,
          y: -(perpLineM * (lineWidth / 2)) / aux + lineP1.y
        }
      } else {
        return { x: p1n1X, y: p1n1Y }
      }
    }

    // Nota: em princícpio não é preciso calcular os pontos todos. Calcular só o ponto inicial e guardar a distância da linha e width
    if (a === 0) {        // isHorizontal
      return { x: lineP1.x, y: lineP1.y - lineWidth / 2 }
    } else if (b === 0) { // isVertical
      return { x: lineP1.x + lineWidth / 2, y: lineP1.y }
    }
  }

  const getDistance = function () {
    const x = Math.sqrt(Math.pow((lineP2.x - lineP1.x), 2) + Math.pow((lineP2.y - lineP1.y), 2))
    return x
  }

  this.initPoint = getRectInitPoint()
  this.width = width
  this.distance = getDistance()

  // representa o retângulo criado no canvas, não o desenhando, para que se possa usar a função isPointInPath
  this.contains = function (currPoint) {
    const x = this.initPoint.x
    const y = this.initPoint.y

    // calculate angle of rectangle
    const rad = calculateRadsWithX(lineP1, lineP2)
    canvasContext.beginPath()
    canvasContext.strokeStyle = 'red'
    canvasContext.save()
    canvasContext.translate(x, y)
    canvasContext.rotate(rad)
    canvasContext.translate(-x, -y)
    // draw rect
    canvasContext.rect(x, y, this.distance, this.width)
    canvasContext.stroke()
    canvasContext.restore()
    if (canvasContext.isPointInPath(currPoint.x, currPoint.y)) {
      return true
    }
    return false
  }

  const calculateRadsWithX = function (lineP1, lineP2) {
    // m = -(   a   ) / (   b   )
    // m = -(Yb - Ya) / (Xb - Xa)
    const a = lineP2.y - lineP1.y
    const b = lineP2.x - lineP1.x
    if (b === 0) {         // isVertical
      return 1.570796     // degree=90º
    } else if (a === 0) { // isHorizontal
      return 0
    }
    const m = a / b
    return Math.atan(m)
  }
}
