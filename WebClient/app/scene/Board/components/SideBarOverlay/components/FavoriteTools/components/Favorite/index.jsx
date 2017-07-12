import React from 'react'
import styles from './styles.scss'
import Pen from './../../../../../../../../model/tools/Pen'
import Eraser from './../../../../../../../../model/tools/Eraser'

import { Button, Icon } from 'semantic-ui-react'

export default class Favorite extends React.Component {
  state = {
    openFavMenu: false
  }
  toggleFavMenu = (evt) => {
    evt.preventDefault()
    this.setState({openFavMenu: !this.state.openFavMenu})
  }
  render () {
    const favMenuWidth = this.state.openFavMenu ? 'visible' : 'hidden'
    const fav = this.props.fav
    // todo: change the way this is done
    let name = null
    let color = 'black'
    if (fav instanceof Pen) {
      name = 'pencil'
      color = fav.color
    } else if (fav instanceof Eraser) {
      name = 'eraser'
    }
    let style = null
    if (this.props.currTool === fav) {
      style = {
        backgroundColor: 'gray'
      }
    }
    return (
      <div onContextMenu={this.toggleFavMenu}>
        <Button circular className={styles.fav} style={style} onClick={() => this.props.changeCurrentTool(fav)}>
          {/* it could be color:red instead of style:{color:'red'}} but the first one does not support rgba */}
          <Icon className={styles.iconStyle + ' large'} name={name} style={{color: color}} />
          <font className={styles.fontStyle} size='1'> {fav.width} </font>
        </Button>
        <Button onClick={() => this.props.removeFavorite(fav)} icon={{name: 'trash', style: {color: 'red'}}} content='Remove' style={{marginLeft: '-25px', visibility: favMenuWidth}} className={styles.favMenu} />
      </div>
    )
  }
}
