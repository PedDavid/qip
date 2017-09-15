import React from 'react'
import styles from './styles.scss'
import Tool from './components/Tool'
import GenericTool from './components/GenericTool'

import {
  Button,
  Icon
} from 'semantic-ui-react'

const margin = 20

export default class Tools extends React.Component {
  state = {
    visible: false,
    top: 0,
    left: 0
  }

  scaleBtnPosition (canvasSize) {
    const width = this.refs.plusDiv.offsetWidth
    return canvasSize.width - width - margin
  }

  onOpenImage = (event) => {
    const file = event.target.files[0]
    if (!file) {
      return
    }
    const reader = new FileReader()
    reader.onload = (event) => {
      this.props.onImageLoad(reader.result)
    }
    reader.readAsDataURL(file)
  }
  toggleTools = () => this.setState(prev => { return {visible: !prev.visible} })

  componentWillReceiveProps (nexProps) {
    // if window size increases, canvas size must be updated
    if (this.scaleBtnPosition(nexProps.canvasSize) !== this.state.left) {
      this.setState({
        left: this.scaleBtnPosition(nexProps.canvasSize)
      })
      return true
    }
    return false
  }

  render () {
    const visibility = this.state.visible ? 'visible' : 'hidden'
    // todo: change the way this is done
    const btnStyle = !this.state.visible
    ? { left: this.state.left }
    : {
      left: this.state.left,
      borderRadius: '50px 50px 0px 0px',
      border: '1px solid #000000'
    }
    return (
      <div ref='plusDiv' className={styles.plusDiv} style={{left: this.state.left}}>
        {/* Plus Button to Open Tools */}
        <Button circular className={styles.plusMenuBtn} style={btnStyle} onClick={this.toggleTools}>
          <Icon className={'plus ' + styles.plusMenuBtnIcon} />
        </Button>
        <div className={styles.plusMenuGrid} style={{visibility}}>
          {/* tool example = {type: pen, content:['black', 'red'] */}
          {this.props.toolsConfig.defaultTools.map((tool, idx) => (
            <Tool key={'tool' + idx} toolsConfig={this.props.toolsConfig} visibility={visibility} currTool={this.props.currTool} grid={this.props.grid} tool={tool} changeCurrentTool={this.props.changeCurrentTool} persist={this.props.persist} />
          ))}
          <GenericTool name='image' onClick={this.props.onImageLoad} />
          <GenericTool name='trash' onClick={this.props.cleanCanvas} />
          <GenericTool name='undo' onClick={this.props.undo} />
        </div>
      </div>
    )
  }
}
