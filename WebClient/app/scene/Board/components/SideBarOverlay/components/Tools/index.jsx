// @flow

import React from 'react'
import styles from './styles.scss'
import Tool from './components/Tool'
import GenericTool from './components/GenericTool'

import {
  Button,
  Grid
} from 'semantic-ui-react'

const halfbtnSize = 20

const defaultTools = [
  {type: 'pencil',
    content: [{value: 'black', size: 'large'}, {value: 'green', size: 'large'}, {value: 'blue', size: 'large'}, {value: 'red', size: 'large'},
      {value: 'yellow', size: 'large'}, {value: 'pink', size: 'large'}, {value: 'grey', size: 'large'}]},
  {type: 'selected radio', content: [{value: '5', size: 'mini'}, {value: '10', size: 'tiny'}, {value: '15', size: 'small'}, {value: '20', size: 'large'}]},
  {type: 'eraser', content: [{value: '5', size: 'mini'}, {value: '10', size: 'tiny'}, {value: '15', size: 'small'}, {value: '20', size: 'large'}]},
  {type: 'move', content: []}
]

export default class Tools extends React.Component {
  state = {
    visible: false,
    top: 25 - halfbtnSize,
    left: 1175 - halfbtnSize
  }
  toggleTools = () => this.setState(prev => { return {visible: !prev.visible} })
  render () {
    const {top, left} = this.state
    const visibility = this.state.visible ? 'visible' : 'hidden'
    const gridDyamicStyle = {
      visibility,
      top: top + halfbtnSize * 2,
      left: left,
      margin: '0px' // todo: would be nice if this was in styles.toolRow but doesnt work, strangely
    }

    return (
      <div>
        {/* Plus Button to Open Tools */}
        <Button circular icon='plus' className={styles.plusMenu} style={{top: this.state.top, left: this.state.left}} onClick={this.toggleTools} />
        <Grid divided textAlign='center' className={styles.plusMenu} style={gridDyamicStyle}>
          {/* tool example = {type: pen, content:['black', 'red'] */}
          {defaultTools.map((tool, idx) => (
            <Grid.Row key={'tool' + idx} columns={1} className={styles.toolRow} style={{padding: '0px'}}>
              <Grid.Column style={{padding: '0px'}}>
                <Tool currTool={this.props.currTool} grid={this.props.grid} tool={tool} changeCurrentTool={this.props.changeCurrentTool} />
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
}
