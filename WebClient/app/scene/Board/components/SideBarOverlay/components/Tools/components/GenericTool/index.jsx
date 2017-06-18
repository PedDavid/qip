// @flow

import React from 'react'
//import styles from './styles.scss'

import {
  Button, 
  Grid,
  Icon
} from 'semantic-ui-react'

export default class GenericTool extends React.Component {
  render () {
    return (
      <div onClick={this.props.onClickTool} style = {{width: '38px', height: '38px'}}>
          <Icon name = {this.props.content} size='large' style = {{paddingTop: '5px', width: '38px', height: '38px'}} />
      </div>
    )
  }
}