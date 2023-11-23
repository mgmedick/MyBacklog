<template>
    <div>
        <div v-if="!indexvm.isAuth">
            <div class="text-center mb-3">
                <h2><span class="me-2">Welcome to mybacklog.io</span><span v-if="indexvm.isDemo" class="text-warning">demo</span></h2>
                <div>
                    <span>A tool to manage your games and keep track of your progress</span>
                </div>
            </div>
            <div v-if="indexvm.isDemo" class="mx-auto" style="max-width: 400px;">
                <form @submit.prevent="submitForm">
                    <div id="divrecaptcha" class="d-flex justify-content-center"></div>
                    <br>
                    <div class="row g-2">
                        <button type="submit" class="btn btn-primary d-flex justify-content-center align-items-center" :disabled="!recaptchaToken">Start Demo</button>
                        <a :href="indexvm.returnUrl" class="btn btn-primary">Back to mybacklog.io</a>
                    </div>
                </form>
            </div>            
            <div v-else id="carouselExampleDark" class="carousel carousel-dark"> 
                <div class="carousel-indicators">
                    <button type="button" data-bs-target="#carouselExampleDark" data-bs-slide-to="0" class="active" aria-current="true" aria-label="Slide 1"></button>
                    <button type="button" data-bs-target="#carouselExampleDark" data-bs-slide-to="1" aria-label="Slide 2"></button>
                    <button type="button" data-bs-target="#carouselExampleDark" data-bs-slide-to="2" aria-label="Slide 3"></button>
                </div>
                <div class="carousel-inner">                    
                    <div class="carousel-item active" data-bs-interval="10000">
                        <img :src="indexImagePath" class="d-block w-75 mx-auto">
                        <div class="carousel-caption">
                            <h5>Manage your games</h5>
                            <p>Add/Remove games from your lists</p>
                        </div>
                    </div>
                    <div class="carousel-item" data-bs-interval="2000">
                        <img :src="importImagePath" class="d-block w-75 mx-auto">
                        <div class="carousel-caption text-light">
                            <h5>Import games</h5>
                            <p>Import games from your linked accounts</p>
                        </div>
                    </div>
                    <div class="carousel-item">
                        <img :src="settingsImagePath" class="d-block w-75 mx-auto">
                        <div class="carousel-caption">
                            <h5>Manage your lists</h5>
                            <p>Add/Edit lists and link accounts</p>
                        </div>
                    </div>
                </div>
                <button class="carousel-control-prev" type="button" data-bs-target="#carouselExampleDark" data-bs-slide="prev">
                    <span class="carousel-control-prev-icon" aria-hidden="true"></span>
                    <span class="visually-hidden">Previous</span>
                </button>
                <button class="carousel-control-next" type="button" data-bs-target="#carouselExampleDark" data-bs-slide="next">
                    <span class="carousel-control-next-icon" aria-hidden="true"></span>
                    <span class="visually-hidden">Next</span>
                </button>
            </div>
        </div>  
        <div v-else>
            <user-lists :userlists="indexvm.userLists" :showimport="indexvm.showImport"></user-lists>
        </div>   
        <div ref="welcomemodal" class="modal modal-lg" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <!-- <h5 class="modal-title">Welcome</h5> -->
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <welcome :username="indexvm.username" :isdemo="indexvm.isDemo"></welcome>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-primary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>
        <div ref="loadingmodal" class="modal" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered justify-content-center" style="color: #fff;">
                <font-awesome-icon icon="fa-solid fa-spinner" spin size="2xl" />
            </div>
        </div>                          
    </div>    
</template>
<script>
    import axios from 'axios';
    import { Modal } from 'bootstrap';
    import { errorToast } from '../../js/common.js';

    export default {
        name: "Index",
        props: {
            indexvm: Object
        },
        data: function () {
            return {
                width: document.documentElement.clientWidth,
                height: document.documentElement.clientHeight
            };
        },         
        computed: { 
        },                 
        data: function () {
            return {
                indexImagePath: '',
                importImagePath: '',
                settingsImagePath: '',
                recaptchaToken: ''
            };
        },
        watch: {},           
        created: function () {
        },
        mounted: function () {
            var that = this;
            
            this.setDemoImages();

            if(this.indexvm.isDemo) {
                this.createGoogleRecaptchaScript().then(function() {
                    grecaptcha.ready(function() {
                        grecaptcha.render('divrecaptcha', {
                            'sitekey' : that.indexvm.recaptchaKey,
                            'callback' : that.onRecaptchaCallback
                        });          
                    });      
                });  
            }          
            
            if (that.indexvm.showWelcome) {
                new Modal(that.$refs.welcomemodal).show();
            }  

            window.addEventListener('resize', this.onResize);
        },  
        destroyed() {
            window.removeEventListener('resize', this.onResize);     
        },                  
        methods: {  
            submitForm() {
                var that = this;
              
                new Modal(this.$refs.loadingmodal).show();
                
                axios.post('/Home/LoginDemo', null, { params: { token: this.recaptchaToken } })
                    .then((res) => {
                        if (res.data.success) {
                            location.href = '/';
                        } else {
                            res.data.errorMessages.forEach(errorMsg => {
                                errorToast(errorMsg);                           
                            }); 
                        }
                        Modal.getInstance(that.$refs.loadingmodal).hide();                                           
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });                
            },      
            onRecaptchaCallback: function(response) {
                this.recaptchaToken = response;
            },            
            onResize: function() {
                var that = this;
                if (that.width != document.documentElement.clientWidth || that.height != document.documentElement.clientHeight) {     
                    that.width = document.documentElement.clientWidth;
                    that.height = document.documentElement.clientHeight;   

                    that.$nextTick(function() {
                        this.setDemoImages();
                    });
                }
            },              
            setDemoImages() {
                var imageSize = '';

                if (this.$el) {
                    var width = (window.innerWidth > 0) ? window.innerWidth : screen.width;

                    if (width >= 992) {
                        imageSize = "lg";
                    } else if (width >= 768) {
                        imageSize = "md";
                    } else if (width >= 576) {
                        imageSize = "xs";
                    } else {
                        imageSize = "xs";
                    }
                }

                this.indexImagePath = this.indexvm.indexDemoImagePath.replace("{0}", imageSize); 
                this.importImagePath = this.indexvm.importDemoImagePath.replace("{0}", imageSize); 
                this.settingsImagePath = this.indexvm.settingsDemoImagePath.replace("{0}", imageSize); 
            },
            createGoogleRecaptchaScript() {
                var that = this;

                return new Promise((resolve, reject) => {
                    let scriptHTML = document.createElement('script');
                    scriptHTML.type = 'text/javascript';
                    scriptHTML.async = true;
                    scriptHTML.defer = true;
                    scriptHTML.src = 'https://www.google.com/recaptcha/api.js?render=explicit';
                    document.getElementsByTagName('head')[0].appendChild(scriptHTML);
                    scriptHTML.onload = function () {
                        resolve();
                    }
                });
            },              
        },
    };
</script>






