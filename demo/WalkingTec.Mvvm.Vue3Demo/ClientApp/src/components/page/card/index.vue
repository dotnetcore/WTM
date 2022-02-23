<template>
	<div>
		<a-card :title="title">
	        <slot name="main"></slot>
	    </a-card>
	</div>
</template>
<script>
	export default {
		data() {
			return {
			}
		},
		props: {
			title:{
				type:String,
				default:''
			}
		},
		methods: {
			
		},
		mounted() {

		}

	}
</script>
<style lang="less" scoped>
    .tarbarList{
        padding-top:0.8rem;
        a{
            color:#777;
            text-align:center;
            margin-bottom:0.8rem;
            display:block;
            div{
                background:#F8F8F8;
                color:#333;
                padding:0.8rem 0;
                font-size:22px;
            }
        }
    }
</style>