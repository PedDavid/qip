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
  }

  shouldComponentUpdate () {
    return false
  }

  render () {
    const {onDown, onUp, onMove, onOut, ...props} = this.props
    return <canvas ref={(canvas) => { this.canvas = canvas }} className={styles.debug} {...props} />
  }
}
