<template>
    <div class="mx-auto">
        <h2 class="text-center mb-3">Import Games</h2>
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
            <div class="row g-2 justify-content-center mb-3">
                <div class="p-0">
                    <input type="radio" class="btn-check" name="options-outlined" id="latest-outlined" autocomplete="off" value="0" v-model="isImportAll">
                    <label class="btn btn-outline-primary me-2" for="latest-outlined">Latest</label>
                    <input type="radio" class="btn-check" name="options-outlined" id="all-outlined" autocomplete="off" value="1" v-model="isImportAll">
                    <label class="btn btn-outline-primary" for="all-outlined">All</label>
                </div>
                <button v-for="(userGameAccount, userGameAccountIndex) in importgamesvm.userGameAccounts" type="button" class="btn btn-outline-dark d-flex justify-content-center align-items-center" @click="onImportGamesClick(userGameAccount)" :disabled="selectedUserGameAccountID">
                    <font-awesome-icon :icon="getIconClass(userGameAccount.gameAccountTypeID)" :class="getIconColorClass(userGameAccount.gameAccountTypeID)" size="xl"/>
                    <div class="mx-auto">
                        <div class="align-self-start">
                            <span class="mx-auto">{{ (selectedUserGameAccountID == userGameAccount.id ? 'Importing ' : 'Import ') + userGameAccount.gameAccountTypeName + ' games' }}</span>
                        </div>
                        <div class="align-self-end text-xs">
                            <span>{{ 'Last imported ' + userGameAccount.relativeImportLastRunDateString }}</span>
                        </div>
                    </div>
                    <font-awesome-icon v-if="selectedUserGameAccountID == userGameAccount.id" icon="fa-solid fa-spinner" spin size="xl"/>
                </button>
            </div>
            <div class="row g-2 justify-content-center">
                <a href="/" class="btn btn-primary mt-3" tabindex="-1" role="button">Back to my games</a>
            </div>
        </div>
    </div>
</template>
<script>
    import { Toast } from 'bootstrap';
    import axios from 'axios';
    
    export default {
        name: "ImportGames",
        props: {
            importgamesvm: Object
        },
        data() {
            return {
                selectedUserGameAccountID: null,   
                isImportAll: 0,           
                successToast: {},
                successMessage: '',
                errorMessages: [],
                errorToast: {}
            }
        },
        computed: {
        },
        created: function () {
        },        
        mounted: function () {
            var that = this;
            that.successToast = new Toast(that.$refs.successtoast);
            that.errorToast = new Toast(that.$refs.errortoast);

            if (that.importgamesvm.authSuccess != null) {
                if (that.importgamesvm.authSuccess) {
                    var userGameAccount = that.importgamesvm.userGameAccounts.find(i => i.gameAccountTypeID == that.importgamesvm.authGameAccountTypeID);
                    that.ImportGames(userGameAccount);
                } else {            
                    that.errorMessages = ["Error authorizing account"];
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
            getIconClass: function (gameAccountTypeID) {
                var iconClass = '';

                switch (gameAccountTypeID) {
                    case 1:
                        iconClass = 'fa-brands fa-steam';
                        break;
                    case 2:
                        iconClass = 'fa-brands fa-xbox';
                        break;
                }

                return iconClass;
            },
            getIconColorClass: function (gameAccountTypeID) {
                var iconClass = '';

                switch (gameAccountTypeID) {
                    case 1:
                        iconClass = 'text-color-blue';
                        break;
                    case 2:
                        iconClass = 'text-color-green';
                        break;
                }

                return iconClass;
            },            
            onImportGamesClick(userGameAccount) {
                this.ImportGames(userGameAccount);
            },
            ImportGames(userGameAccount) {
                var that = this;
                that.selectedUserGameAccountID = userGameAccount.id;

                return axios.post('/Home/ImportGames', null,{ params: { userGameAccountID: userGameAccount.id, isImportAll: that.isImportAll == 1 } })
                    .then((res) => {
                        if (res.data.success) {
                            that.successMessage = "Successfully imported " + userGameAccount.gameAccountTypeName + " games"
                            that.successToast.show();
                        } else {
                            that.errorMessages = ["Error importing " + userGameAccount.gameAccountTypeName + " games"];
                            if (that.errorMessages.length > 0) {
                                that.$nextTick(function() {
                                    that.$refs.errortoasts?.forEach(el => {
                                        new Toast(el).show();
                                    });
                                }); 
                            }                           
                        }
                        that.selectedUserGameAccountID = null;
                    })
                    .catch(err => { console.error(err); return Promise.reject(err); });
            },            
        }
    };
</script>


