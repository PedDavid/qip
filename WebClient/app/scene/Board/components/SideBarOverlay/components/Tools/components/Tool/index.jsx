import React from 'react'
import ToolsSideMenu from './components/ToolsSideMenu'
import Move from './../../../../../../../../model/tools/Move'
import Presentation from './../../../../../../../../model/tools/Presentation'

import {
  Icon
} from 'semantic-ui-react'

export default class Tool extends React.Component {
  state = {sideMenuOpened: false}

  toggleSideMenu = () => {
    // TODO(peddavid): This is because Move Tool doesn't have "subtools",
    //  That said, this logic should not be here at all, or at least generified
    if (this.props.tool.content.length <= 0) {
      let tool
      this.props.tool.type === 'move' && (tool = new Move(this.props.grid))
      this.props.tool.type === 'presentation' && (tool = new Presentation(this.props.grid, this.props.persist))
      this.props.changeCurrentTool(tool)
    } else {
      this.setState(lastState => ({sideMenuOpened: !lastState.sideMenuOpened}))
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
