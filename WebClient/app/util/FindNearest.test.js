import { findNearest, INVALID_IDX } from './Math'

describe('Util: findNearest', () => {
  const identity = element => element

  it('Returns invalid index if empty', () => {
    expect(findNearest([], 0, identity)).toBe(INVALID_IDX)
  })
  it('Returns 0 if array size is 1', () => {
    expect(findNearest([1], 10, identity)).toBe(0)
  })
  it('Returns index of element searched', () => {
    const toFind = 4
    const array = [1, 2, 3, 4, 5, 6, 7, 8, 9]
    const toFindIndex = array.findIndex(element => element === toFind)
    expect(findNearest(array, toFind, identity)).toBe(toFindIndex)
  })
  it('Returns index of last element', () => {
    const toFind = 9
    const array = [1, 2, 3, 4, 5, 6, 7, 8, 9]
    const toFindIndex = array.findIndex(element => element === toFind)
    expect(findNearest(array, toFind, identity)).toBe(toFindIndex)
  })
  it('Returns index of first element', () => {
    const toFind = 1
    const array = [1, 2, 3, 4, 5, 6, 7, 8, 9]
    const toFindIndex = array.findIndex(element => element === toFind)
    expect(findNearest(array, toFind, identity)).toBe(toFindIndex)
  })
  it('Returns index closer element', () => {
    const toFind = 4
    const array = [1, 2, 3, 6, 7, 8, 9]
    const nearestIndex = findNearest(array, toFind, identity)
    const nearestElement = array[nearestIndex]
    expect(nearestElement).toBe(3)
  })
  it('Returns index of closer element in first position', () => {
    const toFind = -1
    const array = [1, 2, 3, 5, 6, 7, 8, 9]
    const nearestIndex = findNearest(array, toFind, identity)
    expect(nearestIndex).toBe(0)
  })
  it('Returns index of closer element in last position', () => {
    const toFind = 10
    const array = [1, 2, 3, 5, 6, 7, 8, 9]
    const nearestIndex = findNearest(array, toFind, identity)
    expect(nearestIndex).toBe(array.length - 1)
  })
})
