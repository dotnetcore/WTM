import { observable } from 'mobx'

const counterStore = observable({
  counter: 0,
  counterStore() {
    this.counter++
  },
  increment() {
    this.counter++
  },
  decrement() {
    this.counter--
  },
  async incrementAsync() {
    await new Promise((res, rej) => {
      setTimeout(() => {
        this.counter++
        res()
      }, 1000)
    })
  }
})
export default counterStore