// @flow

import React from 'react'
import styles from './styles.scss'
import Tool from './components/Tool'
import GenericTool from './components/GenericTool'

import {
  Button,
  Grid,
  Icon
} from 'semantic-ui-react'

const halfbtnSize = 20
const margin = 30

export default class Tools extends React.Component {
  state = {
    visible: false,
    top: 25 - halfbtnSize,
    left: this._scaleBtnPosition(this.props.canvasSize)
  }
  toggleTools = () => this.setState(prev => { return {visible: !prev.visible} })

  componentWillReceiveProps (nexProps) {
    // if window size increases, canvas size must be updated
    if (this._scaleBtnPosition(nexProps.canvasSize) !== this.state.left) {
      this.setState({
        left: this._scaleBtnPosition(nexProps.canvasSize)
      })
      return true
    }
    return false
  }

  render () {
    const {top, left} = this.state
    const visibility = this.state.visible ? 'visible' : 'hidden'
    const gridDyamicStyle = {
      visibility,
      top: top + halfbtnSize * 2,
      left: left,
      margin: '0px' // todo: would be nice if this was in styles.toolRow but doesnt work, strangely
    }
    // todo: change the way this is done
    const btnStyle = !this.state.visible
    ? {
      top: this.state.top,
      left: this.state.left
    }
    : {
      top: this.state.top,
      left: this.state.left,
      borderRadius: '50px 50px 0px 0px',
      border: '1px solid #000000'
    }
    return (
      <div>
        {/* Plus Button to Open Tools */}
        <Button circular className={styles.plusMenu} style={btnStyle} onClick={this.toggleTools}>
          <Icon className='plus' style={{position: 'absolute', top: '14px', right: '13px', margin: '0px'}} />
        </Button>
        <Grid divided textAlign='center' className={styles.plusMenu} style={gridDyamicStyle}>
          {/* tool example = {type: pen, content:['black', 'red'] */}
          {this.props.toolsConfig.defaultTools.map((tool, idx) => (
            <Grid.Row key={'tool' + idx} columns={1} className={styles.toolRow} style={{padding: '0px'}}>
              <Grid.Column style={{padding: '0px'}}>
                <Tool toolsConfig={this.props.toolsConfig} visibility={visibility} currTool={this.props.currTool} grid={this.props.grid} tool={tool} changeCurrentTool={this.props.changeCurrentTool} />
              </Grid.Column>
            </Grid.Row>
          ))}
          <Grid.Row columns={1} className={styles.toolRow} style={{padding: '0px'}}>
            <Grid.Column style={{padding: '0px'}}>
              <GenericTool content='trash' onClickTool={this.props.cleanCanvas} />
            </Grid.Column>
          </Grid.Row>
        </Grid>
      </div>
    )
  }

  _scaleBtnPosition (canvasSize) {
    return canvasSize.width - halfbtnSize - margin
  }
}
