// @flow tool

export interface Tool {
  onPress (id: number, coord: {x:number, y:number}, event: window.Event, scale: number): void;
  onPressUp (id: number, coord: {x:number, y:number}, event: window.Event, scale: number): void;
  onSwipe (id: number, coord: {x:number, y:number}, event: window.Event, scale: number): void;
}
