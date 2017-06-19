// @flow

import React from 'react'
import Favorite from './components/Favorite'
import styles from './styles.scss'
import Pen from './../../../../../../model/tools/Pen'
import Eraser from './../../../../../../model/tools/Eraser'

import { Grid, Button } from 'semantic-ui-react'

export default class FavoriteTools extends React.Component {
  addFavorite = () => {
    const currTool = this.props.currTool
    const favorites = this.props.favorites
    if ((currTool instanceof Pen && favorites.some(tool => tool.width === currTool.width && tool.color === currTool.color)) ||
      (currTool instanceof Eraser && favorites.some(tool => tool.width === currTool.width))) {
      window.alert('Sorry but this tool is already a favorite. Cannot insert duplicates!')
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
                <Favorite changeCurrentTool={this.props.changeCurrentTool} removeFavorite={this.props.removeFavorite} fav={favorite} />
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
