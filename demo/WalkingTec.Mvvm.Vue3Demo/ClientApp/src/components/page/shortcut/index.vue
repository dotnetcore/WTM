<template>
		<a
            :href="href"
            target="_blank"
        >
            <div><span :class="icon"></span></div>
            {{font}}
        </a>
</template>
<script>
	export default {
		data() {
			return {
			}
		},
		props: {
			href:{
				type:String,
				default:''
			},
			font:{
				type:String,
				default:''
			},
			icon:{
				type:String,
				default:''
			},
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