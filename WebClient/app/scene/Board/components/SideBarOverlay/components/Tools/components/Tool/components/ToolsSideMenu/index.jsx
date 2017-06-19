// @flow

import React from 'react'
import styles from './styles.scss'
import Pen from './../../../../../../../../../../model/tools/Pen'
import Eraser from './../../../../../../../../../../model/tools/Eraser'

import {
  Icon
} from 'semantic-ui-react'

export default class ToolsSideMenu extends React.Component {
  changeCurrentTool (toolInstance) {
    this.props.toggleSideMenu() // must be here to avoid a small user experience bug
    const toolInst = toolInstance[0]
    let tool = null
    if (this.props.tool.type === 'pencil') {
      tool = new Pen(this.props.grid, toolInst.value, 5)
    } else if (this.props.tool.type === 'eraser') {
      tool = new Eraser(this.props.grid, toolInst.value)
    } else if (this.props.tool.type === 'selected radio') {
      tool = new Pen(this.props.grid, 'black', toolInst.value)
    }
    this.props.changeCurrentTool(tool)
  }

  render () {
    const toolContent = this.props.tool.content
    const toolType = this.props.tool.type
    const toolContentSize = toolContent.length
    const visibility = this.props.opened ? 'visible' : 'hidden'
    const colorPicker = toolType === 'pencil' ? toolInstance => toolInstance.value : toolInstance => 'black'

    return (
      <div onMouseLeave={this.props.toggleSideMenu} className={styles.toolMenu} style={{width: 40 * toolContentSize, visibility}}>
        {toolContent.map((toolInstance, idx) => (
          <div key={'toolInstance' + idx} className={styles.block} onClick={this.changeCurrentTool.bind(this, [toolInstance])}>
            <span>
              {/* it could be color:red instead of style:{color:'red'}} but the first one does not support rgba */}
              <Icon name={toolType} size={toolInstance.size} style={{paddingTop: '5px', color: colorPicker(toolInstance)}} />
            </span>
          </div>
        ))}
      </div>
    )
  }
}
