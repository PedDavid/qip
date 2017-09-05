import React from 'react'
import styles from './styles.scss'

import { Button, Icon } from 'semantic-ui-react'

export default class Favorite extends React.Component {
  state = {
    openFavMenu: false,
    animateClass: styles.fav
  }
  componentDidMount () {
    this.refs.btnDiv.addEventListener('webkitAnimationEnd', this.endAnimate)
  }
  animate = () => {
    this.setState({animateClass: styles.animateFav})
  }
  endAnimate = () => {
    this.setState({animateClass: ''})
  }
  toggleFavMenu = (evt) => {
    evt.preventDefault()
    this.setState(prevState => ({openFavMenu: !prevState.openFavMenu}))
  }
  render () {
    const favMenuWidth = this.state.openFavMenu ? 'visible' : 'hidden'
    const fav = this.props.fav

    const iconName = this.props.toolsConfig.getDefaultToolOf(fav.type).icon
    const color = fav.color !== undefined ? fav.color : 'black'

    let style = null
    if (this.props.currTool.equals(fav)) {
      style = {
        backgroundColor: 'gray'
      }
    }
    return (
      <div ref='btnDiv' className={this.state.animateClass} onContextMenu={this.toggleFavMenu}>
        <Button circular className={styles.fav} style={style} onClick={() => this.props.changeCurrentTool(fav)}>
          {/* it could be color:red instead of style:{color:'red'}} but the first one does not support rgba */}
          <Icon className={styles.iconStyle + ' large'} name={iconName} style={{color: color}} />
          <font className={styles.fontStyle} size='1'> {fav.width} </font>
        </Button>
        <Button onClick={() => this.props.removeFavorite(fav)}
          icon={{name: 'trash', style: {color: 'red'}}} content=' Remove'
          style={{marginLeft: '-25px', visibility: favMenuWidth}} className={styles.favMenu} />
      </div>
    )
  }
}
