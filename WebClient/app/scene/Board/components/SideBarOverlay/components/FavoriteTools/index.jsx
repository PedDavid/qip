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

  render () {
    return (
      <div className={styles.quickBtnsContainer}>
        <Grid divided textAlign='center'>
          <Grid.Row columns='1' className={styles.rows} style={{padding: '4px'}}> {/* todo: (simaovii) I don't understand why padding in className is not working... */}
            <Grid.Column>
              <Button circular className={styles.btn} icon='list layout' onClick={this.props.toggleSideBar} />
            </Grid.Column>
          </Grid.Row>
          {this.props.favorites.map((favorite, idx) => (
            <Grid.Row key={'favorite' + idx} columns='1' className={styles.rows} style={{padding: '4px'}}>
              <Grid.Column>
                <Favorite ref={'favorite' + idx} toolsConfig={this.props.toolsConfig} currTool={this.props.currTool} changeCurrentTool={this.props.changeCurrentTool} removeFavorite={this.props.removeFavorite} fav={favorite} />
              </Grid.Column>
            </Grid.Row>
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
