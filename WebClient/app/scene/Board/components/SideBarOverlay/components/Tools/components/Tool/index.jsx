import React from 'react'
import ToolsSideMenu from './components/ToolsSideMenu'

import {
  Icon
} from 'semantic-ui-react'

export default class Tool extends React.Component {
  state = {sideMenuOpened: false}

  toggleSideMenu = () => {
    this.setState(lastState => { return {sideMenuOpened: !lastState.sideMenuOpened} })
  }

  render () {
    const tool = this.props.tool
    const opened = this.props.visibility === 'visible' ? this.state.sideMenuOpened : false // if plus button is not opened, close all the sub menus
    return (
      <div onClick={this.toggleSideMenu} style={{width: '38px', height: '38px'}}>
        <Icon name={tool.type} size='large' style={{paddingTop: '5px', width: '38px', height: '38px'}} />
        <ToolsSideMenu {...this.props} toggleSideMenu={this.toggleSideMenu} opened={opened} />
      </div>
    )
  }
}
