<template>
		<a
            :href="infor.href"
            target="_blank"
        >
            <div><span :class="infor.icon"></span></div>
            {{infor.font}}
        </a>
</template>
<script>
	export default {
		data() {
			return {
			}
		},
		props: {
			infor:{
				type:Object,
				default:{}
			}
		},
		methods: {
			
		},
		mounted() {

		}

	}
</script>
<style lang="less" scoped>
	a{
	    color:#777;
	    text-align:center;
	    display:block;
	    div{
	        background:#F8F8F8;
	        color:#333;
	        padding:0.6rem 1rem;
	        font-size:22px;
	        margin-bottom:0.8rem
	    }
	}
    
</style>