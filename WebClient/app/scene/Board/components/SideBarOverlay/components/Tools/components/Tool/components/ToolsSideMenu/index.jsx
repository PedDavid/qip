import React from 'react'
import styles from './styles.scss'
import Pen from './../../../../../../../../../../model/tools/Pen'
import Eraser from './../../../../../../../../../../model/tools/Eraser'

import { Icon } from 'semantic-ui-react'

export default function ToolsSideMenu (props) {
  const changeCurrentTool = (toolInstance) => {
    props.toggleSideMenu() // must be here to avoid a small user experience bug
    const toolInst = toolInstance[0]
    const toolsConfig = props.toolsConfig
    let tool = null

    // check first if currentTool is type of Pen. Otherways, set it to last Pen used
    const pen = props.currTool instanceof Pen ? props.currTool : toolsConfig[props.tool.type].lastValue
    if (props.tool.type === 'pen') {
      tool = new Pen(props.grid, toolInst.value, pen.width)
    } else if (props.tool.type === 'eraser') {
      tool = new Eraser(props.grid, toolInst.value, toolInst.type)
    } else if (props.tool.type === 'width') {
      tool = new Pen(props.grid, pen.color, toolInst.value)
    }
    props.changeCurrentTool(tool)
  }

  const tool = props.tool
  const {content, icon} = tool
  const visibility = props.opened ? 'visible' : 'hidden'

  return (
    <div onMouseLeave={props.toggleSideMenu} className={styles.toolColumnDiv} style={{visibility}}>
      {content.map((toolInstance, idx) => (
        <div key={'toolInstance' + idx} className={styles.tool} onClick={() => changeCurrentTool([toolInstance])}>
          <span>
            {/* it could be color:red instead of style:{color:'red'}} but the first one does not support rgba */}
            <Icon name={icon} size={toolInstance.size} style={{color: toolInstance.color}} className={styles.toolIcon} />
          </span>
        </div>
      ))}
    </div>
  )
}
