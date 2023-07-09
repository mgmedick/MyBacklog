<template>
    <div class="mx-auto">
        <h2 class="text-center mb-1">Welcome to GameStatsApp</h2>
        <div class="mx-auto" style="max-width:400px;">
            <div class="toast-container position-absolute sticky-top p-3 top-0 end-0" id="toastPlacement" style="margin-top: 70px;"> 
                <div ref="errortoasts" v-for="errorMessage in errorMessages" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="d-flex">
                        <div class="toast-body">
                            <span>{{ errorMessage }}</span>
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>           
                <div ref="successtoast" class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="d-flex">
                        <div class="toast-body">
                            <span>{{ successMessage }}</span>
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>
            </div>
            <div class="mb-3">
                <change-username :username="welcomevm.username" @success="onChangeUsernameSuccess" @error="onChangeUsernameError"></change-username>
            </div>
            <div class="text-center">
                <p class="lead text-dark">Link an account to keep your game library up to date automatically</p>
            </div>
            <div class="row g-2 justify-content-center">
                <button type="button" class="btn btn-outline-dark d-flex" :disabled="welcomevm.gameAccountTypeIDs.indexOf(1) > -1"><font-awesome-icon icon="fa-brands fa-steam" size="xl" style="color: #0a3169;" /><span class="mx-auto">Link Steam account</span><font-awesome-icon v-if="welcomevm.gameAccountTypeIDs.indexOf(1) > -1" icon="fa-solid fa-circle-check" size="xl" style="color: #02b875;"/></button>
                <button type="button" class="btn btn-outline-dark d-flex" :disabled="welcomevm.gameAccountTypeIDs.indexOf(2) > -1" @click="onXboxClick"><font-awesome-icon icon="fa-brands fa-xbox" size="xl" style="color: #107711;" /><span class="mx-auto">Link Xbox account</span><font-awesome-icon v-if="welcomevm.gameAccountTypeIDs.indexOf(2) > -1" icon="fa-solid fa-circle-check" size="xl" style="color: #02b875;"/></button>
                <button type="button" class="btn btn-primary" @click="onContinueClick">{{welcomevm.gameAccountTypeIDs.length > 0 ? 'Continue' : 'Skip'}}</button>
            </div>
        </div>
    </div>
</template>
<script>
    import { Toast } from 'bootstrap';
    
    export default {
        name: "Welcome",
        props: {
            welcomevm: Object
        },
        data() {
            return {
                successToast: {},
                successMessage: '',
                errorMessages: [],
                errorToast: {}
            }
        },
        computed: {
        },
        mounted: function () {
            var that = this;
            that.successToast = new Toast(that.$refs.successtoast);
            that.errorToast = new Toast(that.$refs.errortoast);

            if (that.welcomevm.authSuccess != null) {
                if (that.welcomevm.authSuccess) {
                    that.successMessage = "Successfully linked account"
                    that.successToast.show();
                } else {            
                    that.errorMessages = ["Error linking account"];
                    if (that.errorMessages.length > 0) {
                        that.$nextTick(function() {
                            that.$refs.errortoasts?.forEach(el => {
                                new Toast(el).show();
                            });
                        }); 
                    } 
                }
            }            
        },
        methods: {
            onChangeUsernameSuccess(successMsg) {
                that.successMessage = successMsg;
                that.successToast.show();
            },
            onChangeUsernameError(errorMsgs) {
                that.errorMessages = errorMsgs;
                that.$nextTick(function() {
                    that.$refs.errortoasts?.forEach(el => {
                        new Toast(el).show();
                    });
                }); 
            },
            onXboxClick() {
                location.href = this.welcomevm.windowsLiveAuthUrl;
            },            
            onContinueClick() {
                location.href = "/";
            }    
        }
    };
</script>


