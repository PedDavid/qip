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

export default class Tools extends React.Component {
  state = {
    visible: false,
    top: 25 - halfbtnSize,
    left: 1175 - halfbtnSize
  }
  toggleTools = () => this.setState(prev => { return {visible: !prev.visible}})
  render () {
    const {top, left} = this.state
    const visibility = this.state.visible ? 'visible' : 'hidden'
    const gridDyamicStyle = {
      visibility,
      top: this.state.top + halfbtnSize*2,
      left: this.state.left,
      margin: '0px' // todo: would be nice if this was in styles.toolRow but doesnt work, strangely
    }

    return (
      <div>
        {/*Plus Button to Open Tools*/}
        <Button circular icon='plus' className={styles.plusMenu} style={{top: this.state.top, left: this.state.left}} onClick={this.toggleTools}/>
        <Grid divided textAlign='center' className={styles.plusMenu} style={gridDyamicStyle}>
            {this.props.defaultTools.map(tool => (
              <Grid.Row columns={1} className={styles.toolRow} style={{padding:'0px'}}>
                <Grid.Column style={{padding:'0px'}}>
                  <Tool tool={tool}/>
                </Grid.Column>
              </Grid.Row>
            ))}
            <Grid.Row columns={1} className={styles.toolRow} style={{padding:'0px'}}>
                <Grid.Column style={{padding:'0px'}}>
                  <GenericTool content='trash' onClickTool={this.props.cleanCanvas}/>
                </Grid.Column>
              </Grid.Row>
        </Grid>
      </div>
    )
  }
}
