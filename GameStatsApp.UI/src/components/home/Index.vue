<template>
    <div>
        <div v-if="!indexvm.isAuth">
            <div class="text-center mb-3">
                <h2>Welcome to GameStatsApp</h2>
                <div>
                    <span>A tool to keep track of your games and progress</span>
                </div>
            </div>
            <div id="carouselExampleDark" class="carousel carousel-dark"> 
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
            <user-lists :userlists="indexvm.userLists" :emptycoverimagepath="indexvm.emptyCoverImagePath" :useraccounts="indexvm.userAccounts" :windowsliveauthurl="indexvm.windowsLiveAuthUrl" :authsuccess="indexvm.authSuccess" :authaccounttypeid="indexvm.authAccountTypeID"></user-lists>
        </div>   
        <div ref="welcomemodal" class="modal modal-lg" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <!-- <h5 class="modal-title">Welcome</h5> -->
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <welcome :username="indexvm.username"></welcome>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div>               
    </div>    
</template>
<script>
    import { Modal } from 'bootstrap';

    export default {
        name: "Index",
        props: {
            indexvm: Object
        },
        data: function () {
            return {
                welcomeModal: {},
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
                settingsImagePath: ''
            };
        },
        watch: {},
        created: function () {
        },
        mounted: function () {
            var that = this;
            
            window.addEventListener('resize', this.onResize);

            this.setDemoImages();

            that.welcomeModal = new Modal(that.$refs.welcomemodal);         
            
            if (that.indexvm.showWelcome) {
                that.welcomeModal.show();
            }            
        },        
        methods: {
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
            }
        },
    };
</script>






