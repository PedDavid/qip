// @flow

import React from 'react'
import styles from './styles.scss'

import {
  Button, 
  Grid,
  Icon
} from 'semantic-ui-react'

export default class Tool extends React.Component {
  changeCurrentTool () {
    alert('hey')
  }

  render () {
    const tool = this.props.tool

    return (
      <div onClick={this.changeCurrentTool} style = {{width: '38px', height: '38px'}}>
          <Icon name = 'pencil' size='large' style = {{paddingTop: '5px', width: '38px', height: '38px'}} />
      </div>
    )
  }
}