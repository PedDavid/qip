// @flow

import React from 'react'
import styles from './styles.scss'
import Tool from './components/Tool'
import GenericTool from './components/GenericTool'
import Pen from './../../../../../../model/tools/Pen'
import Eraser from './../../../../../../model/tools/Eraser'

import {
  Button,
  Grid
} from 'semantic-ui-react'

const halfbtnSize = 20

// todo: pôr isto tudo num objeto... é uma confusão assim.
// para poder fazer defaultTools.pen ou defaultTools.get(tool : Tool)
// fazer também com que este objeto seja responsável por gerir todas as tools atuais, last tools, etc
const defaultTools = [
  {type: 'pencil',
    content: [{value: 'black', size: 'large'}, {value: 'green', size: 'large'}, {value: 'blue', size: 'large'}, {value: 'red', size: 'large'},
      {value: 'yellow', size: 'large'}, {value: 'pink', size: 'large'}, {value: 'grey', size: 'large'}],
    lastValue: null
  },
  {type: 'selected radio',
    content: [{value: '5', size: 'mini'}, {value: '10', size: 'tiny'}, {value: '15', size: 'small'}, {value: '20', size: 'large'}]
  },
  {type: 'eraser',
    content: [{value: '5', size: 'mini'}, {value: '10', size: 'tiny'}, {value: '15', size: 'small'}, {value: '20', size: 'large'}],
    lastValue: null
  },
  {type: 'move', content: []}
]

export default class Tools extends React.Component {
  state = {
    visible: false,
    top: 25 - halfbtnSize,
    left: 1175 - halfbtnSize
  }
  // this must not be here. When changing favorite tool this must be also called
  updateUsedTools = (newTool) => {
    const prevTool = this.props.currTool
    if (prevTool instanceof Pen) {
      defaultTools[0].lastValue = prevTool
    } else if (prevTool instanceof Eraser) {
      defaultTools[2].lastValue = prevTool
    }
    this.props.changeCurrentTool(newTool)
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
                <Tool defaultTools={defaultTools} visibility={visibility} currTool={this.props.currTool} grid={this.props.grid} tool={tool} changeCurrentTool={this.updateUsedTools} />
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
