// @flow

import React from 'react'
import styles from './styles.scss'
import Tools from './components/Tools'
import FavoriteTools from './components/FavoriteTools'

import {
  Sidebar,
  Segment,
  Menu,
  Icon
} from 'semantic-ui-react'

export default class SideBarOverlay extends React.Component {
  state = {visible: false}
  toggleVisibility = () => this.setState({ visible: !this.state.visible })

  render () {
    const { visible } = this.state
    return (
      <div>
        <Sidebar.Pushable as={Segment} className={styles.sidebar}>
          <Sidebar as={Menu} animation='push' direction='left' width='thin' icon='labeled' visible={visible} vertical inverted>
            <a className='item'>
              <Icon name='home' />
              Home
            </a>
            <a className='item'>
              <Icon name='block layout' />
              Other Icon
            </a>
          </Sidebar>
          <Sidebar.Pusher>
            <Segment basic>
              {this.props.children}
              <FavoriteTools changeCurrentTool={this.props.changeCurrentTool} addFavorite={this.props.addFavorite} currTool={this.props.currTool} favorites={this.props.favorites} toggleSideBar={this.toggleVisibility} />
              <Tools currTool={this.props.currTool} grid={this.props.grid} changeCurrentTool={this.props.changeCurrentTool} defaultTools={this.props.defaultTools} cleanCanvas={this.props.cleanCanvas} />
            </Segment>
          </Sidebar.Pusher>
        </Sidebar.Pushable>
      </div>
    )
  }
}
