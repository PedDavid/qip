import React from 'react'
import Favorite from './components/Favorite'
import styles from './styles.scss'

import { Button } from 'semantic-ui-react'

const favNodes = []
// note: (simao) due to lack of time, i didn't search for a better algorithm without the use of ref. therefore, it mustn't be a statless function
export default class FavoriteTools extends React.Component {
  // TODO(peddavid): Avoid ref usage and auxiliar array, maybe there is an alternative with props
  addFavorite = () => {
    const currTool = this.props.currTool
    const favorites = this.props.favorites
    let idx = -1
    if ((idx = favorites.findIndex(fav => fav.equals(currTool))) > -1) {
      favNodes[idx].animate()
    } else {
      this.props.addFavorite(this.props.currTool)
    }
  }

  onMouseDownFav = (args) => {
    const favIdx = args[0]
    this.div = this.refs['div' + favIdx]
    this.favorite = args[1]
  }
  onMoveFav = (event) => {
    // if event was trigger by hovering, ignore move
    if (event.buttons <= 0 || this.favorite == null) {
      return
    }
    const parentOffsetTop = this.div.offsetTop
    const parentOffsetBottom = parentOffsetTop + this.div.offsetHeight
    const moveY = event.clientY
    // check if move is enough to change favorite position
    if (moveY > parentOffsetBottom) {
      // it's not possible to use "this.props.currTool" because if the second favorite is selected and
      // the user is trying to move the first one, it will throw an error
      // therefore, favorite must be received by parameter
      this.props.moveFavorite(this.favorite, false)
      this.onMouseUp()
    } else if (moveY < parentOffsetTop) {
      this.props.moveFavorite(this.favorite, true)
      this.onMouseUp()
    }
  }
  onMouseUp = (event) => {
    this.favorite = null
    this.div = null
  }

  render () {
    return (
      <div className={styles.favoriteToolsDiv}>
        <Button circular className={styles.favorite + ' ' + styles.favoriteDiv} icon='list layout' onClick={this.props.toggleSideBar} />
        {this.props.favorites.map((favorite, idx) => (
          <div ref={'div' + idx} key={'div' + idx} className={styles.favoriteDiv}
            onMouseUp={this.onMouseUp} onMouseMove={this.onMoveFav} onMouseDown={this.onMouseDownFav.bind(this, [idx, favorite])}>
            <Favorite ref={favNode => { favNodes[idx] = favNode }} toolsConfig={this.props.toolsConfig} currTool={this.props.currTool}
              changeCurrentTool={this.props.changeCurrentTool} removeFavorite={this.props.removeFavorite} fav={favorite}
              moveFavorite={this.props.moveFavorite} />
          </div>
        ))}
        {/* Add Favorite Button */}
        <Button className={styles.favorite + ' ' + styles.favoriteDiv} circular icon='plus' onClick={this.addFavorite} />
      </div>
    )
  }
}
