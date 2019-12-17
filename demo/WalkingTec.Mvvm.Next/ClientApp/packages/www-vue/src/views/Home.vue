<template>
  <div class="home">
    <HelloWorld msg="Welcome to Your Vue.js App"/>
    <grid/>
  </div>
</template>

<script>
// @ is an alias to /src
import HelloWorld from '@/components/HelloWorld.vue'
import grid from '@/components/grid/index.vue'

export default {
  name: 'home',
  components: {
    HelloWorld,
    grid
  }
}
</script>
