import React from 'react'
import styles from './styles.scss'
import {
  Menu,
  Icon
} from 'semantic-ui-react'

export default class ContextMenu extends React.Component {
  render () {
    if (!this.props.visible) {
      return null
    }
    const itemHeight = 35
    let menuHeight = 0
    this.props.contextMenuRaw.forEach(menuSection => {
      menuHeight += itemHeight
      menuSection.menuItems.forEach(menuItem => { menuHeight += itemHeight })
    })

    const dynamicPositionStyle = {
      top: this.props.top > this.props.canvasSize.height - menuHeight ? `${this.props.top - menuHeight}px` : `${this.props.top}px`,
      left: this.props.left > this.props.canvasSize.width - 200 ? `${this.props.left - 200}px` : `${this.props.left}px`
    }
    return (
      <Menu vertical style={dynamicPositionStyle} className={styles.contextMenu}>
        {this.props.contextMenuRaw.map(menuSection => {
          const header = menuSection.header.icon != null
            ? <Menu.Header><Icon name={menuSection.header.icon} />{menuSection.header.text}</Menu.Header>
            : <Menu.Header>{menuSection.header.text}</Menu.Header>
          return (
            <Menu.Item key={menuSection.text}>
              {header}
              <Menu.Menu>
                {menuSection.menuItems.map(item => {
                  return <Menu.Item key={item.text} icon={item.icon} name={item.text} onMouseDown={item.onClick} />
                })}
              </Menu.Menu>
            </Menu.Item>
          )
        })}
      </Menu>
    )
  }
}
