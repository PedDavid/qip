// @flow

import React from 'react'
import styles from './styles.scss'
import Pen from './../../../../../../../../../../model/tools/Pen'
import Eraser from './../../../../../../../../../../model/tools/Eraser'

import {
  Icon
} from 'semantic-ui-react'

export default class ToolsSideMenu extends React.Component {
  changeCurrentTool = (toolInstance) => {
    this.props.toggleSideMenu() // must be here to avoid a small user experience bug
    const toolInst = toolInstance[0]
    const toolsConfig = this.props.toolsConfig
    let tool = null

    // check first if currentTool is type of Pen. Otherways, set it to last Pen used
    const pen = this.props.currTool instanceof Pen ? this.props.currTool : toolsConfig[this.props.tool.type].lastValue
    if (this.props.tool.type === 'pen') {
      tool = new Pen(this.props.grid, toolInst.value, pen.width)
    } else if (this.props.tool.type === 'eraser') {
      tool = new Eraser(this.props.grid, toolInst.value)
    } else if (this.props.tool.type === 'width') {
      tool = new Pen(this.props.grid, pen.color, toolInst.value)
    }
    this.props.changeCurrentTool(tool)
  }

  render () {
    const tool = this.props.tool
    const {content, icon} = tool
    const toolContentSize = content.length
    const visibility = this.props.opened ? 'visible' : 'hidden'

    return (
      <div onMouseLeave={this.props.toggleSideMenu} className={styles.toolMenu} style={{width: 40 * toolContentSize, visibility}}>
        {content.map((toolInstance, idx) => (
          <div key={'toolInstance' + idx} className={styles.block} onClick={() => this.changeCurrentTool([toolInstance])}>
            <span>
              {/* it could be color:red instead of style:{color:'red'}} but the first one does not support rgba */}
              <Icon name={icon} size={toolInstance.size} style={{paddingTop: '5px', color: toolInstance.color}} />
            </span>
          </div>
        ))}
      </div>
    )
  }
}
