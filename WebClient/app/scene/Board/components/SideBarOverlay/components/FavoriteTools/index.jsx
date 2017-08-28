import React from 'react'
import Favorite from './components/Favorite'
import styles from './styles.scss'

import { Grid, Button } from 'semantic-ui-react'

// TODO(peddavid): This could probably be a functional Component
export default class FavoriteTools extends React.Component {
  addFavorite = () => {
    // TODO(peddavid): Is this really needed? Prop by prop comparison should be enough.
    const currTool = this.props.currTool
    const favorites = this.props.favorites
    let idx = -1
    if ((idx = favorites.findIndex(fav => fav.equals(currTool))) > -1) {
      this.refs['favorite' + idx].animate()
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
      <div className={styles.quickBtnsContainer}>
        <Grid divided textAlign='center'>
          <Grid.Row columns='1' className={styles.rows} >
            <Grid.Column>
              <Button circular className={styles.btn} icon='list layout' onClick={this.props.toggleSideBar} />
            </Grid.Column>
          </Grid.Row>
          {this.props.favorites.map((favorite, idx) => (
            <div ref={'div' + idx} key={'div' + idx}> {/* this div allows onMove function to know offset positions of element */}
              <Grid.Row key={'div' + idx} columns='1' className={styles.rows} style={{padding: '4px'}}
                onMouseUp={this.onMouseUp} onMouseMove={this.onMoveFav} onMouseDown={this.onMouseDownFav.bind(this, [idx, favorite])}>
                <Grid.Column>
                  <Favorite ref={'favorite' + idx} toolsConfig={this.props.toolsConfig} currTool={this.props.currTool}
                    changeCurrentTool={this.props.changeCurrentTool} removeFavorite={this.props.removeFavorite} fav={favorite}
                    moveFavorite={this.props.moveFavorite} />
                </Grid.Column>
              </Grid.Row>
            </div>
          ))}
          {/* Add Favorite Button */}
          <Grid.Row columns='1' className={styles.rows} style={{padding: '4px'}}>
            <Grid.Column>
              <Button className={styles.btn} circular icon='plus' onClick={this.addFavorite} />
            </Grid.Column>
          </Grid.Row>
        </Grid>
      </div>
    )
  }
}
