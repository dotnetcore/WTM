<template>
		<a
            :href="hrefEnd"
            v-if="type == 'Self'"
        >
            <div><span :class="icon"></span></div>
            {{font}}
        </a>

        <a
            :href="hrefEnd"
            target="_target"
            v-if="type == 'Target'"
            
        >
            <div><span :class="icon"></span></div>
            {{font}}
        </a>
        
        <a
            @click="visible = true"
            href="javascript:;"
            v-if="type == 'Dialog'"
        >
            <div><span :class="icon"></span></div>
            {{font}}
        </a>

        <a-modal
		    class="w-view"
		    footer
		    :visible="visible"
		    width="1000px"
		    :title="font"
		    destroyOnClose
		    @cancel="visible = false" 
		  >
			<iframe width='100%'  frameborder='0' height="600" scrolling='auto' :src='hrefEnd'></iframe>	 
	   </a-modal>
</template>
<script>
	export default {
		data() {
			return {
				visible:false,
				hrefEnd:null
			}
		},
		props: {
			type:{
				type:String,
				default:'Self'
			},
			href:{
				type:String,
				default:''
			},
			target: {
				type: String,
				default:'self'
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
			this.hrefEnd = this.href.indexOf("https://") > -1 || this.href.indexOf("http://") > -1 ? this.href :this.href+"?jumptype=Dixlog"
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
	        font-size:40px;
	        margin-bottom:0.8rem
	    }
	}
    
</style>
