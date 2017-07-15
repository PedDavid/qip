// @flow

import React from 'react'
import {
  Link
} from 'react-router-dom'
import styles from './styles.scss'
import Tools from './components/Tools'
import FavoriteTools from './components/FavoriteTools'

import {
  Sidebar,
  Segment,
  Menu,
  Icon,
  Dropdown,
  Button
} from 'semantic-ui-react'

export default class SideBarOverlay extends React.Component {
  state = {
    visible: false,
    extraStyle: null
  }
  toggleVisibility = () => this.setState({ visible: !this.state.visible })

  openBoards = () => {
    console.log(styles.myBoards)
    this.setState({extraStyle: {
      paddingRight: '300px',
      width: '210px'
    }})
  }

  closeBoards = () => {
    this.setState({extraStyle: {
      paddingRight: '0px',
      width: '150px'
    }})
  }

  render () {
    const { visible } = this.state

    return (
      <div>
        <Sidebar.Pushable as={Segment} className={styles.sidebar}>
          <Sidebar ref='sidebar' as={Menu} animation='push' direction='left' width='thin' icon='labeled' visible={visible} vertical style={this.state.extraStyle} inverted className={styles.sidebarMenu}>
            <Link onClick={this.props.toggleUserModal} to='/signin' className='item'>
              <Icon name='sign in' />
              Login
            </Link>
            <a className='item'>
              <Icon name='share' />
              Share board
            </a>
            <Dropdown ref='menux' onClose={this.closeBoards} onClick={this.openBoards} item text='My Boards'>
              <Dropdown.Menu className={styles.myBoards} >
                <Dropdown.Header>School</Dropdown.Header>
                <Dropdown.Item>Semester 1 xpto class</Dropdown.Item>
                <Dropdown.Item>Semester 2</Dropdown.Item>
                <Dropdown.Item>Semester 3</Dropdown.Item>

                <Dropdown.Header>Personal</Dropdown.Header>
                <Dropdown.Item >Diary</Dropdown.Item>
                <Dropdown.Item >Ideas</Dropdown.Item>
                <Dropdown.Item >House</Dropdown.Item>

                <Dropdown.Item className={styles.addBoard}>
                  <Button className={styles.btnPlus}>
                    <Icon name='plus' className={styles.iconPlus} />
                  </Button>
                </Dropdown.Item>                
              </Dropdown.Menu>
            </Dropdown>
            <a className='item'>
              <Icon name='share' />
              New Board
            </a>
            {/* Only Appears if current scretch is not associated with a board */}
            <a className='item'>
              <Icon name='save' />
              Save Board
            </a>
          </Sidebar>
          <Sidebar.Pusher>
            <Segment basic>
              {this.props.children}
              <FavoriteTools toolsConfig={this.props.toolsConfig} changeCurrentTool={this.props.changeCurrentTool} removeFavorite={this.props.removeFavorite} addFavorite={this.props.addFavorite} currTool={this.props.currTool} favorites={this.props.favorites} toggleSideBar={this.toggleVisibility} />
              <Tools currTool={this.props.currTool} grid={this.props.grid} changeCurrentTool={this.props.changeCurrentTool} toolsConfig={this.props.toolsConfig} cleanCanvas={this.props.cleanCanvas} />
            </Segment>
          </Sidebar.Pusher>
        </Sidebar.Pushable>
      </div>
    )
  }
}
