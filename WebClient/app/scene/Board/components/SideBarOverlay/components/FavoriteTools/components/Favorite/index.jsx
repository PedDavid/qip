import React from 'react'
import styles from './styles.scss'

import { Button, Icon } from 'semantic-ui-react'

export default class Favorite extends React.Component {
  state = {
    openFavMenu: false
  }
  toggleFavMenu = (evt) => {
    evt.preventDefault()
    this.setState({openFavMenu: !this.state.openFavMenu})
  }
  removeFavorite = () => {
    const fav = this.props.fav
    this.props.removeFavorite(fav)
  }
  changeCurrentTool = () => {
    const fav = this.props.fav
    this.props.changeCurrentTool(fav)
  }
  render () {
    const favMenuWidth = this.state.openFavMenu ? 'visible' : 'hidden'
    const fav = this.props.fav

    const iconName = this.props.toolsConfig.getDefaultToolOf(fav.constructor.name).icon
    const color = fav.color !== undefined ? fav.color : 'black'

    let style = null
    if (this.props.currTool === fav) {
      style = {
        backgroundColor: 'gray'
      }
    }
    return (
      <div onContextMenu={this.toggleFavMenu}>
        <Button circular className={styles.fav} style={style} onClick={this.changeCurrentTool}>
          {/* it could be color:red instead of style:{color:'red'}} but the first one does not support rgba */}
          <Icon className={styles.iconStyle + ' large'} name={iconName} style={{color: color}} />
          <font className={styles.fontStyle} size='1'> {fav.width} </font>
        </Button>
        <Button onClick={this.removeFavorite} icon={{name: 'trash', style: {color: 'red'}}} content='Remove' style={{marginLeft: '-25px', visibility: favMenuWidth}} className={styles.favMenu} />
      </div>
    )
  }
}
