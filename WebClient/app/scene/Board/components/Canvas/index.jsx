// @flow

import React from 'react'

type PointerEvents = {
  onDown?: any => void,
  onUp?: any => void,
  onMove?: any => void,
  onOut?: any => void
}

export default class Canvas extends React.Component {
  props: PointerEvents
  canvas: window.EventListener

  componentDidMount () {
    if (this.props.onDown !== null) {
      this.canvas.addEventListener('pointerdown', this.props.onDown, false)
    }
    if (this.props.onUp !== null) {
      this.canvas.addEventListener('pointerup', this.props.onUp, false)
    }
    if (this.props.onMove !== null) {
      this.canvas.addEventListener('pointermove', this.props.onMove, false)
    }
    if (this.props.onOut !== null) {
      this.canvas.addEventListener('pointerout', this.props.onOut, false)
    }
  }

  shouldComponentUpdate () {
    return false
  }

  render () {
    const { onDown, onUp, onMove, onOut, ...props } = this.props
    const style = { border: '1px solid #000000' } // TODO(PedDavid): Delete this, used only for debug purposes
    return <canvas ref={canvas => { this.canvas = canvas }} {...props} style={style} />
  }
}
