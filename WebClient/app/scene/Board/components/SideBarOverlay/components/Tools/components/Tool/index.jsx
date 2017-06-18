// @flow

import React from 'react'
import ToolsSideMenu from './components/ToolsSideMenu'

import {
  Icon
} from 'semantic-ui-react'

export default class Tool extends React.Component {
  state = {sideMenuOpened: false}

  toggleSideMenu () {
    this.setState(lastState => { return {sideMenuOpened: !lastState.sideMenuOpened} })
  }

  render () {
    const tool = this.props.tool

    return (
      <div onClick={this.toggleSideMenu.bind(this)} style={{width: '38px', height: '38px'}}>
        <Icon name={tool.type} size='large' style={{paddingTop: '5px', width: '38px', height: '38px'}} />
        <ToolsSideMenu {...this.props} opened={this.state.sideMenuOpened} />
      </div>
    )
  }
}
