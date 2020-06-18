<template>
  <div class="container">
    <el-row>
      <el-col :span="12" class="align-self-center">
        <svg version="1.1" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 0 800 600" style="visibility: visible;">
          <g>
            <defs>
              <clippath id="GlassClip">
                <path d="M380.857,346.164c-1.247,4.651-4.668,8.421-9.196,10.06c-9.332,3.377-26.2,7.817-42.301,3.5
                s-28.485-16.599-34.877-24.192c-3.101-3.684-4.177-8.66-2.93-13.311l7.453-27.798c0.756-2.82,3.181-4.868,6.088-5.13
                c6.755-0.61,20.546-0.608,41.785,5.087s33.181,12.591,38.725,16.498c2.387,1.682,3.461,4.668,2.705,7.488L380.857,346.164z"></path>
              </clippath>
              <clippath id="cordClip">
                <rect width="800" height="600"></rect>
              </clippath>
            </defs>

            <g id="planet" transform="matrix(0.9994,-0.0336,0.0336,0.9994,-3.315,19.1486)" style="transform-origin: 0px 0px;">
              <circle fill="none" stroke="#0E0620" stroke-width="3" stroke-miterlimit="10" cx="572.859" cy="108.803" r="90.788"></circle>

              <circle id="craterBig" fill="none" stroke="#0E0620" stroke-width="3" stroke-miterlimit="10" cx="548.891" cy="62.319" r="13.074" transform="matrix(1,0,0,1,2.892,0)" style="transform-origin: 0px 0px;"></circle>

              <circle id="craterSmall" fill="none" stroke="#0E0620" stroke-width="3" stroke-miterlimit="10" cx="591.743" cy="158.918" r="7.989" transform="matrix(1,0,0,1,-2.892,0)" style="transform-origin: 0px 0px;"></circle>
              <path id="ring" fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" d="
			M476.562,101.461c-30.404,2.164-49.691,4.221-49.691,8.007c0,6.853,63.166,12.408,141.085,12.408s141.085-5.555,141.085-12.408
			c0-3.378-15.347-4.988-40.243-7.225"></path>

              <path id="ringShadow" opacity="0.5" fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" d="
			M483.985,127.43c23.462,1.531,52.515,2.436,83.972,2.436c36.069,0,68.978-1.19,93.922-3.149"></path>
            </g>
            <g id="stars">
              <g id="starsBig">
                <g transform="matrix(0.9977,-0.0677,0.0677,0.9977,-16.1382,35.6621)" style="transform-origin: 0px 0px;">

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="518.07" y1="245.375" x2="518.07" y2="266.581"></line>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="508.129" y1="255.978" x2="528.01" y2="255.978"></line>
                </g>
                <g transform="matrix(0.9991,0.0412,-0.0412,0.9991,10.1093,-6.1496)" style="transform-origin: 0px 0px;">

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="154.55" y1="231.391" x2="154.55" y2="252.598"></line>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="144.609" y1="241.995" x2="164.49" y2="241.995"></line>
                </g>
                <g transform="matrix(0.9989,-0.0471,0.0471,0.9989,-6.3996,15.236)" style="transform-origin: 0px 0px;">

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="320.135" y1="132.746" x2="320.135" y2="153.952"></line>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="310.194" y1="143.349" x2="330.075" y2="143.349"></line>
                </g>
                <g transform="matrix(0.9978,-0.0669,0.0669,0.9978,-32.5879,14.511)" style="transform-origin: 0px 0px;">

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="200.67" y1="483.11" x2="200.67" y2="504.316"></line>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="210.611" y1="493.713" x2="190.73" y2="493.713"></line>
                </g>
              </g>
              <g id="starsSmall">
                <g transform="matrix(1,0,0,1,0,0)" style="transform-origin: 0px 0px;">

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="432.173" y1="380.52" x2="432.173" y2="391.83"></line>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="426.871" y1="386.175" x2="437.474" y2="386.175"></line>
                </g>
                <g transform="matrix(1,0,0,1,0,0)" style="transform-origin: 0px 0px;">

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="489.555" y1="299.765" x2="489.555" y2="308.124"></line>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="485.636" y1="303.945" x2="493.473" y2="303.945"></line>
                </g>
                <g transform="matrix(1,0,0,1,0,0)" style="transform-origin: 0px 0px;">

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="231.468" y1="291.009" x2="231.468" y2="299.369"></line>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="227.55" y1="295.189" x2="235.387" y2="295.189"></line>
                </g>
                <g transform="matrix(1,0,0,1,0,0)" style="transform-origin: 0px 0px;">

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="244.032" y1="547.539" x2="244.032" y2="555.898"></line>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="247.95" y1="551.719" x2="240.113" y2="551.719"></line>
                </g>
                <g transform="matrix(0.998,0,0,0.9982,0.3727,0.7401)" style="transform-origin: 0px 0px;">

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="186.359" y1="406.967" x2="186.359" y2="415.326"></line>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="190.277" y1="411.146" x2="182.44" y2="411.146"></line>
                </g>
                <g transform="matrix(0.941,0,0,0.9414,28.3375,24.0932)" style="transform-origin: 0px 0px;">

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="480.296" y1="406.967" x2="480.296" y2="415.326"></line>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" x1="484.215" y1="411.146" x2="476.378" y2="411.146"></line>
                </g>
              </g>
              <g id="circlesBig">

                <circle fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" cx="588.977" cy="255.978" r="7.952" transform="matrix(1,0,0,1,0,-1.928)" style="transform-origin: 0px 0px;"></circle>

                <circle fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" cx="450.066" cy="320.259" r="7.952" transform="matrix(1,0,0,1,0,-1.928)" style="transform-origin: 0px 0px;"></circle>

                <circle fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" cx="168.303" cy="353.753" r="7.952" transform="matrix(1,0,0,1,0,-1.928)" style="transform-origin: 0px 0px;"></circle>

                <circle fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" cx="429.522" cy="201.185" r="7.952" transform="matrix(1,0,0,1,0,-1.928)" style="transform-origin: 0px 0px;"></circle>

                <circle fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" cx="200.67" cy="176.313" r="7.952" transform="matrix(1,0,0,1,0,-1.928)" style="transform-origin: 0px 0px;"></circle>

                <circle fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" cx="133.343" cy="477.014" r="7.952" transform="matrix(1,0,0,1,0,-1.928)" style="transform-origin: 0px 0px;"></circle>

                <circle fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" cx="283.521" cy="568.033" r="7.952" transform="matrix(1,0,0,1,0,-1.928)" style="transform-origin: 0px 0px;"></circle>

                <circle fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-miterlimit="10" cx="413.618" cy="482.387" r="7.952" transform="matrix(1,0,0,1,0,-1.928)" style="transform-origin: 0px 0px;"></circle>
              </g>
              <g id="circlesSmall">
                <circle fill="#0E0620" cx="549.879" cy="296.402" r="2.651" transform="matrix(1,0,0,1,0,-3.857)" style="transform-origin: 0px 0px;"></circle>
                <circle fill="#0E0620" cx="253.29" cy="229.24" r="2.651" transform="matrix(1,0,0,1,0,-3.857)" style="transform-origin: 0px 0px;"></circle>
                <circle fill="#0E0620" cx="434.824" cy="263.931" r="2.651" transform="matrix(1,0,0,1,0,-3.857)" style="transform-origin: 0px 0px;"></circle>
                <circle fill="#0E0620" cx="183.708" cy="544.176" r="2.651" transform="matrix(1,0,0,1,0,-3.857)" style="transform-origin: 0px 0px;"></circle>
                <circle fill="#0E0620" cx="382.515" cy="530.923" r="2.651" transform="matrix(1,0,0,1,0,-3.857)" style="transform-origin: 0px 0px;"></circle>
                <circle fill="#0E0620" cx="130.693" cy="305.608" r="2.651" transform="matrix(1,0,0,1,0,-3.857)" style="transform-origin: 0px 0px;"></circle>
                <circle fill="#0E0620" cx="480.296" cy="477.014" r="2.651" transform="matrix(1,0,0,1,0,-3.857)" style="transform-origin: 0px 0px;"></circle>
              </g>
            </g>
            <g id="spaceman" clip-path="url(cordClip)" transform="matrix(0.9999,0.0168,-0.0168,0.9999,3.7074,1.3827)" style="transform-origin: 0px 0px;">
              <path id="cord" fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
			M273.813,410.969c0,0-54.527,39.501-115.34,38.218c-2.28-0.048-4.926-0.241-7.841-0.548
			c-68.038-7.178-134.288-43.963-167.33-103.87c-0.908-1.646-1.793-3.3-2.654-4.964c-18.395-35.511-37.259-83.385-32.075-118.817"></path>

              <path id="backpack" fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
			M338.164,454.689l-64.726-17.353c-11.086-2.972-17.664-14.369-14.692-25.455l15.694-58.537
			c3.889-14.504,18.799-23.11,33.303-19.221l52.349,14.035c14.504,3.889,23.11,18.799,19.221,33.303l-15.694,58.537
			C360.647,451.083,349.251,457.661,338.164,454.689z"></path>
              <g id="antenna">
                <line fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" x1="323.396" y1="236.625" x2="295.285" y2="353.753"></line>
                <circle fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" cx="323.666" cy="235.617" r="6.375"></circle>
              </g>
              <g id="armR">

                <path fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
				M360.633,363.039c1.352,1.061,4.91,5.056,5.824,6.634l27.874,47.634c3.855,6.649,1.59,15.164-5.059,19.02l0,0
				c-6.649,3.855-15.164,1.59-19.02-5.059l-5.603-9.663"></path>

                <path fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
				M388.762,434.677c5.234-3.039,7.731-8.966,6.678-14.594c2.344,1.343,4.383,3.289,5.837,5.793
				c4.411,7.596,1.829,17.33-5.767,21.741c-7.596,4.411-17.33,1.829-21.741-5.767c-1.754-3.021-2.817-5.818-2.484-9.046
				C375.625,437.355,383.087,437.973,388.762,434.677z"></path>
              </g>
              <g id="armL">

                <path fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
				M301.301,347.66c-1.702,0.242-5.91,1.627-7.492,2.536l-47.965,27.301c-6.664,3.829-8.963,12.335-5.134,18.999h0
				c3.829,6.664,12.335,8.963,18.999,5.134l9.685-5.564"></path>

                <path fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
				M241.978,395.324c-3.012-5.25-2.209-11.631,1.518-15.977c-2.701-0.009-5.44,0.656-7.952,2.096
				c-7.619,4.371-10.253,14.09-5.883,21.71c4.371,7.619,14.09,10.253,21.709,5.883c3.03-1.738,5.35-3.628,6.676-6.59
				C252.013,404.214,245.243,401.017,241.978,395.324z"></path>
              </g>
              <g id="body">

                <path fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
				M353.351,365.387c-7.948,1.263-16.249,0.929-24.48-1.278c-8.232-2.207-15.586-6.07-21.836-11.14
				c-17.004,4.207-31.269,17.289-36.128,35.411l-1.374,5.123c-7.112,26.525,8.617,53.791,35.13,60.899l0,0
				c26.513,7.108,53.771-8.632,60.883-35.158l1.374-5.123C371.778,395.999,365.971,377.536,353.351,365.387z"></path>
                <path fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
				M269.678,394.912L269.678,394.912c26.3,20.643,59.654,29.585,93.106,25.724l2.419-0.114"></path>
              </g>
              <g id="legs">
                <g id="legR">

                  <path fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
					M312.957,456.734l-14.315,53.395c-1.896,7.07,2.299,14.338,9.37,16.234l0,0c7.07,1.896,14.338-2.299,16.234-9.37l17.838-66.534
					C333.451,455.886,323.526,457.387,312.957,456.734z"></path>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" x1="304.883" y1="486.849" x2="330.487" y2="493.713"></line>
                </g>
                <g id="legL">

                  <path fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
					M296.315,452.273L282,505.667c-1.896,7.07-9.164,11.265-16.234,9.37l0,0c-7.07-1.896-11.265-9.164-9.37-16.234l17.838-66.534
					C278.993,441.286,286.836,447.55,296.315,452.273z"></path>

                  <line fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" x1="262.638" y1="475.522" x2="288.241" y2="482.387"></line>
                </g>
              </g>
              <g id="head">

                <ellipse transform="matrix(0.259 -0.9659 0.9659 0.259 -51.5445 563.2371)" fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" cx="341.295" cy="315.211" rx="61.961" ry="60.305"></ellipse>
                <path id="headStripe" fill="none" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
				M330.868,261.338c-7.929,1.72-15.381,5.246-21.799,10.246" transform="matrix(0.9999,0.0168,-0.0168,0.9999,4.4214,-4.6842)" style="transform-origin: 0px 0px;"></path>

                <path fill="#FFFFFF" stroke="#0E0620" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" stroke-miterlimit="10" d="
				M380.857,346.164c-1.247,4.651-4.668,8.421-9.196,10.06c-9.332,3.377-26.2,7.817-42.301,3.5s-28.485-16.599-34.877-24.192
				c-3.101-3.684-4.177-8.66-2.93-13.311l7.453-27.798c0.756-2.82,3.181-4.868,6.088-5.13c6.755-0.61,20.546-0.608,41.785,5.087
				s33.181,12.591,38.725,16.498c2.387,1.682,3.461,4.668,2.705,7.488L380.857,346.164z"></path>
                <g clip-path="url(#GlassClip)">
                  <polygon id="glassShine" fill="none" stroke="#0E0620" stroke-width="3" stroke-miterlimit="10" points="
					278.436,375.599 383.003,264.076 364.393,251.618 264.807,364.928 				" transform="matrix(0.866,-0.5,0.5,0.866,-33.401,203.976)" style="transform-origin: 0px 0px;"></polygon>
                </g>
              </g>
            </g>
          </g>
        </svg>
      </el-col>
      <el-col :span="12" class="align-self-center">
        <h1>404</h1>
        <h2>UH OH! 页面丢失</h2>
        <p>您所寻找的页面不存在。你可以点击下面的按钮，返回主页。
        </p>
        <a href="/index.html">
          <el-button type="primary">返回首页</el-button>
        </a>
      </el-col>
    </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
@Component({
    name: "404"
})
export default class extends Vue {}
</script>

<style lang="less" scoped>
svg {
    width: 100%;
    visibility: hidden;
}

h1 {
    font-size: 7.5em;
    margin: 15px 0px;
    font-weight: bold;
}

h2 {
    font-weight: bold;
}

.hamburger-menu {
    position: absolute;
    top: 0;
    left: 0;
    padding: 35px;
    z-index: 2;
}
.hamburger-menu button {
    position: relative;
    width: 30px;
    height: 22px;
    border: none;
    background: none;
    padding: 0;
    cursor: pointer;
}
.hamburger-menu button span {
    position: absolute;
    height: 3px;
    background: #000;
    width: 100%;
    left: 0px;
    top: 0px;
    -webkit-transition: 0.1s ease-in;
    transition: 0.1s ease-in;
}
.hamburger-menu button span:nth-child(2) {
    top: 9px;
}
.hamburger-menu button span:nth-child(3) {
    top: 18px;
}
.hamburger-menu [data-state="open"] span:first-child {
    -webkit-transform: rotate(45deg);
    transform: rotate(45deg);
    top: 10px;
}
.hamburger-menu [data-state="open"] span:nth-child(2) {
    width: 0%;
    opacity: 0;
}
.hamburger-menu [data-state="open"] span:nth-child(3) {
    -webkit-transform: rotate(-45deg);
    transform: rotate(-45deg);
    top: 10px;
}

nav {
    position: absolute;
    height: 100%;
    top: 0;
    left: 0;
    background: var(--green);
    color: var(--blue);
    width: 300px;
    z-index: 1;
    padding-top: 80px;
    -webkit-transform: translateX(-100%);
    transform: translateX(-100%);
    -webkit-transition: 0.24s cubic-bezier(0.52, 0.01, 0.8, 1);
    transition: 0.24s cubic-bezier(0.52, 0.01, 0.8, 1);
}
nav li {
    -webkit-transform: translateX(-5px);
    transform: translateX(-5px);
    -webkit-transition: 0.16s cubic-bezier(0.44, 0.09, 0.46, 0.84);
    transition: 0.16s cubic-bezier(0.44, 0.09, 0.46, 0.84);
    opacity: 0;
}
nav a {
    display: block;
    font-size: 1.75em;
    font-weight: bold;
    text-decoration: none;
    color: inherit;
    -webkit-transition: 0.24s ease-in-out;
    transition: 0.24s ease-in-out;
}
nav a:hover {
    text-decoration: none;
    color: var(--white);
}
nav[data-state="open"] {
    -webkit-transform: translateX(0%);
    transform: translateX(0%);
}
nav[data-state="open"] ul li:nth-child(1) {
    -webkit-transition-delay: 0.16s;
    transition-delay: 0.16s;
    -webkit-transform: translateX(0px);
    transform: translateX(0px);
    opacity: 1;
}
nav[data-state="open"] ul li:nth-child(2) {
    -webkit-transition-delay: 0.32s;
    transition-delay: 0.32s;
    -webkit-transform: translateX(0px);
    transform: translateX(0px);
    opacity: 1;
}
nav[data-state="open"] ul li:nth-child(3) {
    -webkit-transition-delay: 0.48s;
    transition-delay: 0.48s;
    -webkit-transform: translateX(0px);
    transform: translateX(0px);
    opacity: 1;
}
nav[data-state="open"] ul li:nth-child(4) {
    -webkit-transition-delay: 0.64s;
    transition-delay: 0.64s;
    -webkit-transform: translateX(0px);
    transform: translateX(0px);
    opacity: 1;
}

.btn {
    z-index: 1;
    overflow: hidden;
    background: transparent;
    position: relative;
    padding: 8px 50px;
    border-radius: 30px;
    cursor: pointer;
    font-size: 1em;
    letter-spacing: 2px;
    -webkit-transition: 0.2s ease;
    transition: 0.2s ease;
    font-weight: bold;
    margin: 5px 0px;
}
.btn.green {
    border: 4px solid var(--green);
    color: var(--blue);
}
.btn.green:before {
    content: "";
    position: absolute;
    left: 0;
    top: 0;
    width: 0%;
    height: 100%;
    background: var(--green);
    z-index: -1;
    -webkit-transition: 0.2s ease;
    transition: 0.2s ease;
}
.btn.green:hover {
    color: var(--white);
    background: var(--green);
    -webkit-transition: 0.2s ease;
    transition: 0.2s ease;
}
.btn.green:hover:before {
    width: 100%;
}

.container {
    width: 100%;
    padding-right: 15px;
    padding-left: 15px;
    margin-right: auto;
    margin-left: auto;
}
@media (min-width: 576px) {
    .container {
        max-width: 540px;
    }
}
@media (min-width: 768px) {
    .container {
        max-width: 720px;
    }
}
@media (min-width: 992px) {
    .container {
        max-width: 960px;
    }
}
@media (min-width: 1200px) {
    .container {
        max-width: 1140px;
    }
}

@media screen and (max-width: 768px) {
    body {
        display: block;
    }

    .container {
        margin-top: 70px;
        margin-bottom: 70px;
    }
}
</style>
