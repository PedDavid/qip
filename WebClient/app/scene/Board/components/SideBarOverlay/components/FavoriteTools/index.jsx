import React from 'react'
import Favorite from './components/Favorite'
import styles from './styles.scss'

import { Grid, Button } from 'semantic-ui-react'

export default function FavoriteTools (props) {
  // TODO(peddavid): Avoid ref usage and auxiliar array, maybe there is an alternative with props
  const favNodes = []
  const addFavorite = () => {
    const currTool = props.currTool
    const favorites = props.favorites
    let idx = -1
    if ((idx = favorites.findIndex(fav => fav.equals(currTool))) > -1) {
      favNodes[idx].animate()
    } else {
      props.addFavorite(props.currTool)
    }
  }
  return (
    <div className={styles.quickBtnsContainer}>
      <Grid divided textAlign='center'>
        <Grid.Row columns='1' className={styles.rows} style={{padding: '4px'}}> {/* todo: (simaovii) I don't understand why padding in className is not working... */}
          <Grid.Column>
            <Button circular className={styles.btn} icon='list layout' onClick={props.toggleSideBar} />
          </Grid.Column>
        </Grid.Row>
        {props.favorites.map((favorite, idx) => (
          <Grid.Row key={'favorite' + idx} columns='1' className={styles.rows} style={{padding: '4px'}}>
            <Grid.Column>
              <Favorite ref={favNode => { favNodes[idx] = favNode }} toolsConfig={props.toolsConfig} currTool={props.currTool} changeCurrentTool={props.changeCurrentTool} removeFavorite={props.removeFavorite} fav={favorite} />
            </Grid.Column>
          </Grid.Row>
        ))}
        {/* Add Favorite Button */}
        <Grid.Row columns='1' className={styles.rows} style={{padding: '4px'}}>
          <Grid.Column>
            <Button className={styles.btn} circular icon='plus' onClick={addFavorite} />
          </Grid.Column>
        </Grid.Row>
      </Grid>
    </div>
  )
}
