import React from 'react'
// import { Link } from 'react-router-dom'
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

  extendMenu = (padding) => {
    this.setState({extraStyle: {
      paddingRight: padding + 'px',
      width: '210px'
    }})
  }

  closeBoards = () => {
    this.setState({extraStyle: {
      paddingRight: '0px',
      width: '150px'
    }})
  }

  getAuthView () {
    const auth = this.props.auth
    const authUserIcon = auth.isAuthenticated() ? 'user outline' : 'sign in'
    const authLabel = auth.isAuthenticated() ? /* auth.tryGetProfile().sub */ 'Alexandre' : 'Login'
    const authOnClick = auth.isAuthenticated() ? () => this.extendMenu(220) : () => this.props.auth.login()

    const trigger = (
      <span>
        <a style={{maxWidth: '150px'}} onClick={authOnClick} className='item'>
          <Icon name={authUserIcon} />
          {authLabel}
        </a>
      </span>
    )

    const options = [
      { key: 'user', text: 'Account', icon: 'user', onClick: () => window.alert(2) },
      { key: 'settings', text: 'Settings', icon: 'settings', onClick: () => window.alert(2) },
      { key: 'sign-out', text: 'Sign Out', icon: 'sign out', onClick: () => auth.logout() }
    ]

    return auth.isAuthenticated()
    ? <Dropdown onClose={this.closeBoards} trigger={trigger} options={options} pointing='left' icon={null} />
    : trigger
  }

  render () {
    const { visible } = this.state

    return (
      <div>
        <Sidebar.Pushable as={Segment} className={styles.sidebar}>
          <Sidebar ref='sidebar' as={Menu} animation='push' direction='left' width='thin' icon='labeled' visible={visible} vertical style={this.state.extraStyle} inverted className={styles.sidebarMenu}>
            {this.getAuthView()}
            <a onClick={this.props.toggleShareModal} className='item'>
              <Icon name='share' />
              Share board
            </a>
            <Dropdown ref='menux' onClose={this.closeBoards} onClick={() => this.extendMenu(300)} item text='My Boards'>
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
            {/* Only Appears if current scretch is not associated with a board */}
            <a onClick={this.props.toggleSaveModal} className='item'>
              <Icon name='save' />
              Save Board
            </a>
          </Sidebar>
          <Sidebar.Pusher>
            <Segment basic>
              {this.props.children}
              <FavoriteTools toolsConfig={this.props.toolsConfig} changeCurrentTool={this.props.changeCurrentTool} removeFavorite={this.props.removeFavorite} addFavorite={this.props.addFavorite} currTool={this.props.currTool} favorites={this.props.favorites} toggleSideBar={this.toggleVisibility} />
              <Tools currTool={this.props.currTool} grid={this.props.grid} changeCurrentTool={this.props.changeCurrentTool} toolsConfig={this.props.toolsConfig} cleanCanvas={this.props.cleanCanvas} drawImage={this.props.drawImage} canvasSize={this.props.canvasSize} />
            </Segment>
          </Sidebar.Pusher>
        </Sidebar.Pushable>
      </div>
    )
  }
}
