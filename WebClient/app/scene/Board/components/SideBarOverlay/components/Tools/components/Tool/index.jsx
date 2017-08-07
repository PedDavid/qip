import React from 'react'
import ToolsSideMenu from './components/ToolsSideMenu'
import Move from './../../../../../../../../model/tools/Move'

import {
  Icon
} from 'semantic-ui-react'

export default class Tool extends React.Component {
  state = {sideMenuOpened: false}

  toggleSideMenu = () => {
    if (this.props.tool.type === 'move') {
      const tool = new Move(this.props.grid)
      this.props.changeCurrentTool(tool)
    } else {
      this.setState(lastState => { return {sideMenuOpened: !lastState.sideMenuOpened} })
    }
  }

  render () {
    const tool = this.props.tool
    const opened = this.props.visibility === 'visible' && this.state.sideMenuOpened // if plus button is not opened, close all the sub menus
    return (
      <div onClick={this.toggleSideMenu} style={{width: '38px', height: '38px'}}>
        <Icon name={tool.icon} size='large' style={{paddingTop: '5px', width: '38px', height: '38px'}} />
        <ToolsSideMenu toolsConfig={this.props.toolsConfig} {...this.props} toggleSideMenu={this.toggleSideMenu} opened={opened} />
      </div>
    )
  }
}
