import React from 'react'
import styles from './styles.scss'

export default class Canvas extends React.Component {
  componentDidMount () {
    if (this.props.onDown != null) {
      this.canvas.addEventListener('pointerdown', this.props.onDown, false)
    }
    if (this.props.onUp != null) {
      this.canvas.addEventListener('pointerup', this.props.onUp, false)
    }
    if (this.props.onMove != null) {
      this.canvas.addEventListener('pointermove', this.props.onMove, false)
    }
    if (this.props.onOut != null) {
      this.canvas.addEventListener('pointerout', this.props.onOut, false)
    }
    if (this.props.onContextMenu != null) {
      this.canvas.addEventListener('oncontextmenu', this.props.onContextMenu, false)
    }
  }

  shouldComponentUpdate (nexProps) {
    // if window size increases, canvas size must be updated
    if (nexProps.width > this.props.width || nexProps.height > this.props.height) {
      return true
    }
    return false
  }

  render () {
    const dynamicCanvasSize = {
      height: this.props.height - 2,
      width: this.props.width - 2
    }
    const {onDown, onUp, onMove, onOut, ...props} = this.props
    return <canvas ref={(canvas) => { this.canvas = canvas }} className={styles.debug} style={dynamicCanvasSize} {...props} />
  }
}
