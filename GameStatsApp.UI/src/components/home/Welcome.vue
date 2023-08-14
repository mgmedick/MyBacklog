<template>
    <div class="mx-auto">
        <h2 class="text-center mb-1">Welcome to GameStatsApp</h2>
        <div class="mx-auto" style="max-width:400px;">
            <div id="toastPlacement" ref="toastcontainer" class="toast-container position-fixed top-0 end-0" style="margin-top:70px;"> 
                <div ref="errortoast" class="toast align-items-center text-white bg-danger border-0" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="d-flex">
                        <div class="toast-body">
                            <span class="msg-text"></span>
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>              
                <div ref="successtoast" class="toast align-items-center text-white bg-success border-0" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="d-flex">
                        <div class="toast-body">
                            <span class="msg-text"></span>
                        </div>
                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                </div>                         
            </div>
            <div class="mb-3">
                <change-username :username="welcomevm.username"></change-username>
            </div>
            <div class="text-center">
                <p class="lead text-dark">Link an account to keep your game library up to date automatically</p>
            </div>
            <div class="row g-2 justify-content-center mb-3">
                <button type="button" class="btn btn-outline-dark d-flex" :disabled="welcomevm.accountTypeIDs.indexOf(1) > -1"><font-awesome-icon icon="fa-brands fa-steam" size="xl" style="color: #0a3169;" /><span class="mx-auto">Link Steam account</span><font-awesome-icon v-if="welcomevm.accountTypeIDs.indexOf(1) > -1" icon="fa-solid fa-circle-check" size="xl" style="color: #02b875;"/></button>
                <button type="button" class="btn btn-outline-dark d-flex" :disabled="welcomevm.accountTypeIDs.indexOf(2) > -1" @click="onXboxClick"><font-awesome-icon icon="fa-brands fa-xbox" size="xl" style="color: #107711;" /><span class="mx-auto">Link Xbox account</span><font-awesome-icon v-if="welcomevm.accountTypeIDs.indexOf(2) > -1" icon="fa-solid fa-circle-check" size="xl" style="color: #02b875;"/></button>
            </div>
            <div class="row g-2 justify-content-center">
                <button type="button" class="btn btn-primary" @click="onContinueClick">{{welcomevm.accountTypeIDs.length > 0 ? 'Continue' : 'Skip'}}</button>
            </div>
        </div>
    </div>
</template>
<script>
    import { successToast, errorToast } from '../../js/common.js';
    
    export default {
        name: "Welcome",
        props: {
            welcomevm: Object
        },
        data() {
            return {
                successMessage: '',
                errorMessages: [],
            }
        },
        computed: {
        },
        mounted: function () {
            var that = this;

            if (that.welcomevm.authSuccess != null) {
                if (that.welcomevm.authSuccess) {
                    successToast("Successfully linked account");             
                } else {                 
                    errorToast("Error linking account");             
                }
            }            
        },
        methods: {            
            onXboxClick() {
                location.href = this.welcomevm.windowsLiveAuthUrl;
            },            
            onContinueClick() {
                location.href = "/";
            }    
        }
    };
</script>


